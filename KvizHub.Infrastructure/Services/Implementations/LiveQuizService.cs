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
using Microsoft.AspNetCore.SignalR;
using KvizHub.Infrastructure.Hubs;

namespace KvizHub.Infrastructure.Services.Implementations
{
    public class LiveQuizService : ILiveQuizService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<LiveQuizHub> _hubContext;

        public LiveQuizService(ApplicationDbContext context, IHubContext<LiveQuizHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Kreiranje sobe sa nasumičnim kodom
        public async Task<LiveQuizRoom> CreateRoomAsync(CreateLiveQuizDTO createDto, int hostUserId)
        {
            // Proveri da li kviz postoji
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == createDto.QuizId);

            if (quiz == null)
                throw new Exception("Kviz nije pronađen");

            if (quiz.Questions.Count == 0)
                throw new Exception("Kviz nema pitanja");

            // Generiši jedinstveni kod sobe
            var roomCode = GenerateRoomCode();

            var room = new LiveQuizRoom
            {
                RoomCode = roomCode,
                RoomName = createDto.RoomName,
                Description = createDto.Description,
                QuizId = createDto.QuizId,
                HostUserId = hostUserId,
                MaxParticipants = createDto.MaxParticipants,
                ScheduledStart = createDto.ScheduledStart,
                QuestionTimeLimit = createDto.QuestionTimeLimit,
                Status = "Waiting",
                CreatedAt = DateTime.UtcNow
            };

            _context.LiveQuizRooms.Add(room);
            await _context.SaveChangesAsync();

            return room;
        }

        public async Task<LiveQuizRoom> GetRoomByCodeAsync(string roomCode)
        {
            return await _context.LiveQuizRooms
                .Include(r => r.Quiz)
                .Include(r => r.HostUser)
                .Include(r => r.CurrentQuestion)
                .FirstOrDefaultAsync(r => r.RoomCode == roomCode && r.IsActive);
        }

        public async Task<List<LiveQuizRoom>> GetActiveRoomsAsync()
        {
            return await _context.LiveQuizRooms
                .Where(r => r.IsActive && r.Status != "Completed")
                .Include(r => r.Quiz)
                .Include(r => r.HostUser)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // Pridruživanje sobi
        public async Task<LiveQuizParticipant> JoinRoomAsync(string roomCode, int userId, string connectionId)
        {
            var room = await GetRoomByCodeAsync(roomCode);
            if (room == null)
                throw new Exception("Soba nije pronađena");

            if (room.Status != "Waiting")
                throw new Exception("Kviz je već počeo ili završen");

            // Proveri broj učesnika
            var participantCount = await _context.LiveQuizParticipants
                .CountAsync(p => p.LiveQuizRoomId == room.Id && p.IsConnected);

            if (participantCount >= room.MaxParticipants)
                throw new Exception("Soba je puna");

            // Proveri da li je korisnik već u sobi
            var existingParticipant = await _context.LiveQuizParticipants
                .FirstOrDefaultAsync(p => p.LiveQuizRoomId == room.Id && p.UserId == userId);

            if (existingParticipant != null)
            {
                // Ažuriraj connection ID ako se ponovo konektuje
                existingParticipant.ConnectionId = connectionId;
                existingParticipant.IsConnected = true;
                await _context.SaveChangesAsync();
                return existingParticipant;
            }

            // Kreiraj novog učesnika
            var participant = new LiveQuizParticipant
            {
                UserId = userId,
                LiveQuizRoomId = room.Id,
                ConnectionId = connectionId,
                Score = 0,
                CorrectAnswers = 0,
                JoinedAt = DateTime.UtcNow,
                IsConnected = true
            };

            _context.LiveQuizParticipants.Add(participant);
            await _context.SaveChangesAsync();

            return participant;
        }

        // Slanje odgovora sa BONUS BODOVIMA za brzinu
        public async Task<LiveQuizResult> SubmitAnswerAsync(string roomCode, string connectionId, int questionId,
            List<int> selectedAnswerIds, string answerText)
        {
            var room = await GetRoomByCodeAsync(roomCode);
            if (room == null || room.CurrentQuestionId != questionId)
                throw new Exception("Pitanje nije aktivno");

            var participant = await _context.LiveQuizParticipants
                .FirstOrDefaultAsync(p => p.ConnectionId == connectionId && p.IsConnected);

            if (participant == null)
                throw new Exception("Učesnik nije pronađen");

            // Proveri tačnost odgovora (pojednostavljena verzija)
            var question = await _context.Questions
                .Include(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == questionId);

            bool isCorrect = CheckAnswer(question, selectedAnswerIds, answerText);

            // Izračunaj vreme odgovora
            var responseTime = DateTime.UtcNow - room.QuestionStartTime.Value;
            var responseTimeSeconds = responseTime.TotalSeconds;

            // Izračunaj bodove
            int basePoints = isCorrect ? question.Points : 0;
            int speedBonus = CalculateSpeedBonus(isCorrect, responseTimeSeconds, room.QuestionTimeLimit);
            int totalPoints = basePoints + speedBonus;

            // Ažuriraj rezultat učesnika
            if (isCorrect)
            {
                participant.Score += totalPoints;
                participant.CorrectAnswers += 1;
            }

            await _context.SaveChangesAsync();

            return new LiveQuizResult
            {
                IsCorrect = isCorrect,
                PointsEarned = totalPoints,
                BasePoints = basePoints,
                SpeedBonus = speedBonus,
                ResponseTimeSeconds = responseTimeSeconds
            };
        }

        // BONUS SISTEM: Više bodova za brže odgovore
        private int CalculateSpeedBonus(bool isCorrect, double responseTimeSeconds, int timeLimit)
        {
            if (!isCorrect) return 0;

            // Bonus bodovi: što brže odgovoriš, više bonus bodova dobijaš
            double timeRatio = responseTimeSeconds / timeLimit;
            if (timeRatio <= 0.3) return 5;  // Vrlo brzo: +5 bonus
            if (timeRatio <= 0.6) return 3;  // Brzo: +3 bonus
            if (timeRatio <= 0.8) return 1;  // Normalno: +1 bonus
            return 0; // Sporo: nema bonus
        }

        private bool CheckAnswer(Question question, List<int> selectedAnswerIds, string answerText)
        {
            // POJEDNOSTAVLJENA verzija - kasnije ćemo doraditi
            var correctAnswers = question.AnswerOptions
                .Where(a => a.IsCorrect)
                .Select(a => a.Id)
                .ToList();

            return correctAnswers.Count == selectedAnswerIds.Count &&
                   correctAnswers.All(ca => selectedAnswerIds.Contains(ca));
        }

        private string GenerateRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<bool> LeaveRoomAsync(string roomCode, string connectionId)
        {
            var participant = await _context.LiveQuizParticipants
                .FirstOrDefaultAsync(p => p.ConnectionId == connectionId && p.IsConnected);

            if (participant != null)
            {
                participant.IsConnected = false;
                await _context.SaveChangesAsync();

                // Obavesti ostale u sobi
                await _hubContext.Clients.Group(roomCode).SendAsync("UserLeft", connectionId);
                return true;
            }

            return false;
        }

        public async Task<List<LiveQuizParticipant>> GetRoomParticipantsAsync(string roomCode)
        {
            var room = await GetRoomByCodeAsync(roomCode);
            if (room == null) return new List<LiveQuizParticipant>();

            return await _context.LiveQuizParticipants
                .Where(p => p.LiveQuizRoomId == room.Id && p.IsConnected)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task StartQuizAsync(string roomCode, int hostUserId)
        {
            var room = await GetRoomByCodeAsync(roomCode);
            if (room == null || room.HostUserId != hostUserId)
                throw new Exception("Samo host može pokrenuti kviz");

            room.Status = "InProgress";
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(roomCode).SendAsync("QuizStarted");
        }

        public async Task StartQuestionAsync(string roomCode, int questionId, int hostUserId)
        {
            var room = await GetRoomByCodeAsync(roomCode);
            if (room == null || room.HostUserId != hostUserId)
                throw new Exception("Samo host može pokrenuti pitanje");

            room.CurrentQuestionId = questionId;
            room.QuestionStartTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(roomCode).SendAsync("QuestionStarted", new
            {
                QuestionId = questionId,
                StartTime = room.QuestionStartTime,
                TimeLimit = room.QuestionTimeLimit
            });
        }

        public async Task EndQuestionAsync(string roomCode, int hostUserId)
        {
            var room = await GetRoomByCodeAsync(roomCode);
            if (room == null || room.HostUserId != hostUserId)
                throw new Exception("Samo host može završiti pitanje");

            room.CurrentQuestionId = null;
            room.QuestionStartTime = null;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(roomCode).SendAsync("QuestionEnded");
        }

        public async Task<LiveQuizSummary> GetQuizSummaryAsync(string roomCode)
        {
            var room = await GetRoomByCodeAsync(roomCode);
            if (room == null) return null;

            var participants = await GetRoomParticipantsAsync(roomCode);

            return new LiveQuizSummary
            {
                RoomCode = room.RoomCode,
                RoomName = room.RoomName,
                TotalQuestions = room.Quiz.Questions.Count,
                CurrentQuestion = room.CurrentQuestionId.HasValue ?
                    room.Quiz.Questions.OrderBy(q => q.Order).ToList().IndexOf(
                        room.Quiz.Questions.First(q => q.Id == room.CurrentQuestionId.Value)) + 1 : 0,
                Participants = participants.Select(p => new LiveQuizParticipantDTO
                {
                    UserId = p.UserId,
                    Username = p.User.Username,
                    ProfileImage = p.User.ProfileImage,
                    Score = p.Score,
                    CorrectAnswers = p.CorrectAnswers,
                    IsConnected = p.IsConnected
                }).ToList()
            };
        }

        public async Task<List<LiveQuizParticipant>> GetLeaderboardAsync(string roomCode)
        {
            var room = await GetRoomByCodeAsync(roomCode);
            if (room == null) return new List<LiveQuizParticipant>();

            return await _context.LiveQuizParticipants
                .Where(p => p.LiveQuizRoomId == room.Id)
                .Include(p => p.User)
                .OrderByDescending(p => p.Score)
                .ThenByDescending(p => p.CorrectAnswers)
                .ToListAsync();
        }

        public async Task<bool> CloseRoomAsync(string roomCode, int hostUserId)
        {
            var room = await GetRoomByCodeAsync(roomCode);
            if (room == null || room.HostUserId != hostUserId)
                return false;

            room.Status = "Completed";
            room.IsActive = false;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(roomCode).SendAsync("QuizCompleted");
            return true;
        }
        public async Task<List<LiveQuizParticipant>> GetRoomParticipantsByConnectionIdAsync(string connectionId)
        {
            return await _context.LiveQuizParticipants
                .Where(p => p.ConnectionId == connectionId && p.IsConnected)
                .Include(p => p.LiveQuizRoom)
                .ToListAsync();
        }
    }
}
