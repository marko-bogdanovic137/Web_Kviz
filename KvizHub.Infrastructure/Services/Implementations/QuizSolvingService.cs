using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KvizHub.Infrastructure.Services.Interfaces;
using KvizHub.Models.DTOs;
using KvizHub.Models.Entities;
using KvizHub.Infrastructure.Data;

namespace KvizHub.Infrastructure.Services.Implementations
{
    public class QuizSolvingService : IQuizSolvingService
    {
        private readonly ApplicationDbContext _context;

        public QuizSolvingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<QuizDetailDTO> StartQuizAsync(int quizId)
        {
            var quiz = await _context.Quizzes
                .Where(q => q.Id == quizId && q.IsActive)
                .Include(q => q.Category)
                .Include(q => q.Questions)
                    .ThenInclude(qu => qu.AnswerOptions)
                .FirstOrDefaultAsync();

            if (quiz == null) throw new Exception("Kviz nije pronađen");

            return new QuizDetailDTO
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                TimeLimit = quiz.TimeLimit,
                Difficulty = quiz.Difficulty,
                CategoryName = quiz.Category.Name,
                QuestionCount = quiz.Questions.Count,
                Questions = quiz.Questions.OrderBy(q => q.Order)
                    .Select(qu => new QuestionDetailDTO
                    {
                        Id = qu.Id,
                        Text = qu.Text,
                        Type = qu.Type,
                        Points = qu.Points,
                        Order = qu.Order,
                        AnswerOptions = qu.AnswerOptions.OrderBy(a => a.Order)
                            .Select(ao => new AnswerOptionDTO
                            {
                                Id = ao.Id,
                                Text = ao.Text,
                                // NE prikazujemo koji su tačni odgovori!
                                IsCorrect = false, // Sakrijemo korisniku
                                Order = ao.Order
                            }).ToList()
                    }).ToList()
            };
        }

        public async Task<QuizResult> SubmitQuizAsync(int quizId, int userId, QuizSubmissionDTO submission)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(qu => qu.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == quizId && q.IsActive);

            if (quiz == null) throw new Exception("Kviz nije pronađen");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("Korisnik nije pronađen");

            if (submission.TimeSpent > quiz.TimeLimit * 60)
                throw new Exception("Vreme za kviz je prekoračeno");

            if (submission.Answers.Count != quiz.Questions.Count)
                throw new Exception("Broj odgovora se ne poklapa sa brojem pitanja");

            int score = 0;
            var userAnswers = new List<UserAnswer>();

            foreach (var question in quiz.Questions)
            {
                var userAnswerDto = submission.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                if (userAnswerDto == null) continue;

                bool isCorrect = CheckAnswer(question, userAnswerDto);
                int pointsEarned = isCorrect ? question.Points : 0;
                score += pointsEarned;

                // Kreiraj UserAnswer BEZ QuestionId (jer ga model nema)
                var userAnswerEntity = new UserAnswer
                {
                    IsCorrect = isCorrect,
                    AnswerText = userAnswerDto.AnswerText,
                    // QuizResultId će biti postavljeno kasnije
                    SelectedAnswers = new List<SelectedAnswer>()
                };

                // Dodaj SelectedAnswers za multiple choice
                foreach (var answerId in userAnswerDto.SelectedAnswerIds)
                {
                    userAnswerEntity.SelectedAnswers.Add(new SelectedAnswer
                    {
                        AnswerOptionId = answerId
                    });
                }

                userAnswers.Add(userAnswerEntity);
            }

            // Kreiraj QuizResult
            var quizResult = new QuizResult
            {
                UserId = userId,
                QuizId = quizId,
                Score = score,
                TotalQuestions = quiz.Questions.Count,
                Percentage = (double)score / quiz.Questions.Count * 100,
                TimeSpent = submission.TimeSpent,
                CompletedAt = DateTime.UtcNow,
                UserAnswers = userAnswers // Ovo će automatski povezati UserAnswer sa QuizResult
            };

            _context.QuizResults.Add(quizResult);
            await _context.SaveChangesAsync();

            return quizResult;
        }

        private bool CheckAnswer(Question question, QuestionAnswerDTO userAnswer)
        {
            return question.Type.ToLower() switch
            {
                "multiplechoice" => CheckMultipleChoice(question, userAnswer),
                "truefalse" => CheckTrueFalse(question, userAnswer),
                "fillintheblank" => CheckFillInBlank(question, userAnswer),
                _ => false
            };
        }

        private bool CheckMultipleChoice(Question question, QuestionAnswerDTO userAnswer)
        {
            var correctAnswers = question.AnswerOptions.Where(a => a.IsCorrect).Select(a => a.Id).ToList();
            var userSelected = userAnswer.SelectedAnswerIds;

            // Provera da li su isti skupovi
            return correctAnswers.Count == userSelected.Count &&
                   correctAnswers.All(ca => userSelected.Contains(ca));
        }

        private bool CheckTrueFalse(Question question, QuestionAnswerDTO userAnswer)
        {
            // TrueFalse je zapravo MultipleChoice sa 2 opcije
            return CheckMultipleChoice(question, userAnswer);
        }

        private bool CheckFillInBlank(Question question, QuestionAnswerDTO userAnswer)
        {
            var correctAnswer = question.AnswerOptions.First().Text.ToLower().Trim();
            var userAnswerText = (userAnswer.AnswerText ?? "").ToLower().Trim();

            return correctAnswer == userAnswerText;
        }

        public async Task<QuizResultDTO> GetQuizResultAsync(int resultId)
        {
            var result = await _context.QuizResults
                .Include(qr => qr.Quiz)
                .Include(qr => qr.UserAnswers)
                    .ThenInclude(ua => ua.SelectedAnswers)
                .FirstOrDefaultAsync(qr => qr.Id == resultId);

            if (result == null) return null;

            return await MapQuizResultToDTO(result);
        }

        public async Task<List<QuizResultDTO>> GetUserQuizHistoryAsync(int userId)
        {
            var results = await _context.QuizResults
                .Where(qr => qr.UserId == userId)
                .Include(qr => qr.Quiz)
                .OrderByDescending(qr => qr.CompletedAt)
                .ToListAsync();

            var resultDTOs = new List<QuizResultDTO>();
            foreach (var result in results)
            {
                resultDTOs.Add(await MapQuizResultToDTO(result));
            }

            return resultDTOs;
        }

        private async Task<QuizResultDTO> MapQuizResultToDTO(QuizResult result)
        {
            var dto = new QuizResultDTO
            {
                Id = result.Id,
                QuizId = result.QuizId,
                QuizTitle = result.Quiz.Title,
                Score = result.Score,
                TotalQuestions = result.TotalQuestions,
                Percentage = result.Percentage,
                TimeSpent = result.TimeSpent,
                CompletedAt = result.CompletedAt,
                UserAnswers = new List<UserAnswerDTO>()
            };

            // Popuni UserAnswers detalje
            foreach (var userAnswer in result.UserAnswers)
            {
                // Pronađi pitanje preko SelectedAnswers → AnswerOption → Question
                var selectedAnswer = userAnswer.SelectedAnswers.FirstOrDefault();
                if (selectedAnswer == null) continue;

                // Učitaj AnswerOption sa Question
                var answerOption = await _context.AnswerOptions
                    .Include(ao => ao.Question)
                    .FirstOrDefaultAsync(ao => ao.Id == selectedAnswer.AnswerOptionId);

                if (answerOption?.Question == null) continue;

                var question = answerOption.Question;

                var userAnswerDTO = new UserAnswerDTO
                {
                    QuestionText = question.Text,
                    QuestionType = question.Type,
                    IsCorrect = userAnswer.IsCorrect,
                    PointsEarned = userAnswer.IsCorrect ? question.Points : 0,
                    CorrectAnswers = question.AnswerOptions
                        .Where(a => a.IsCorrect)
                        .Select(a => a.Text)
                        .ToList(),
                    SelectedAnswers = new List<string>()
                };

                // Popuni selektovane odgovore
                foreach (var selectedAns in userAnswer.SelectedAnswers)
                {
                    var answer = await _context.AnswerOptions
                        .FirstOrDefaultAsync(a => a.Id == selectedAns.AnswerOptionId);
                    if (answer != null)
                        userAnswerDTO.SelectedAnswers.Add(answer.Text);
                }

                // Za fill-in-the-blank dodaj tekstualni odgovor
                if (!string.IsNullOrEmpty(userAnswer.AnswerText))
                    userAnswerDTO.SelectedAnswers.Add(userAnswer.AnswerText);

                dto.UserAnswers.Add(userAnswerDTO);
            }

            return dto;
        }

        private class AnswerData
        {
            public List<int> SelectedAnswerIds { get; set; } = new();
            public string AnswerText { get; set; } = string.Empty;
        }
    }
}