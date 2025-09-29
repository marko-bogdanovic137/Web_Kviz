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
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Korisnici
        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    ProfileImage = u.ProfileImage,
                    CreatedAt = u.CreatedAt
                })
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ToggleUserStatusAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            // Možemo dodati IsActive property kasnije ako treba
            // Za sada samo vraćamo true - kasnije ćemo implementirati
            return true;
        }

        // Kvizovi
        public async Task<List<QuizAdminDTO>> GetAllQuizzesAsync(bool includeInactive = false) // Promeni povratni tip
        {
            var query = _context.Quizzes.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(q => q.IsActive);
            }

            return await query
                .Include(q => q.Category)
                .Include(q => q.CreatedByUser)
                .Include(q => q.Questions)
                .Select(q => new QuizAdminDTO
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    TimeLimit = q.TimeLimit,
                    Difficulty = q.Difficulty,
                    IsActive = q.IsActive,
                    CategoryName = q.Category.Name,
                    CreatedByUsername = q.CreatedByUser.Username,
                    QuestionCount = q.Questions.Count,
                    CreatedAt = q.CreatedByUser.CreatedAt
                })
                .OrderByDescending(q => q.Id)
                .ToListAsync();
        }

        public async Task<Quiz> CreateQuizAsync(CreateQuizDTO createQuizDto, int adminUserId)
        {
            var quiz = new Quiz
            {
                Title = createQuizDto.Title,
                Description = createQuizDto.Description,
                TimeLimit = createQuizDto.TimeLimit,
                Difficulty = createQuizDto.Difficulty,
                CategoryId = createQuizDto.CategoryId,
                CreatedByUserId = adminUserId,
                IsActive = true,
                Questions = createQuizDto.Questions.Select((q, qIndex) => new Question
                {
                    Text = q.Text,
                    Type = q.Type,
                    Points = q.Points,
                    Order = q.Order,
                    AnswerOptions = q.AnswerOptions.Select((a, aIndex) => new AnswerOption
                    {
                        Text = a.Text,
                        IsCorrect = a.IsCorrect,
                        Order = a.Order
                    }).ToList()
                }).ToList()
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<Quiz> UpdateQuizAsync(int quizId, CreateQuizDTO updateQuizDto)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(qu => qu.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null) return null;

            // Ažuriraj osnovne podatke
            quiz.Title = updateQuizDto.Title;
            quiz.Description = updateQuizDto.Description;
            quiz.TimeLimit = updateQuizDto.TimeLimit;
            quiz.Difficulty = updateQuizDto.Difficulty;
            quiz.CategoryId = updateQuizDto.CategoryId;

            // Obriši postojeća pitanja i odgovore
            _context.AnswerOptions.RemoveRange(quiz.Questions.SelectMany(q => q.AnswerOptions));
            _context.Questions.RemoveRange(quiz.Questions);

            // Dodaj nova pitanja
            quiz.Questions = updateQuizDto.Questions.Select((q, qIndex) => new Question
            {
                Text = q.Text,
                Type = q.Type,
                Points = q.Points,
                Order = q.Order,
                AnswerOptions = q.AnswerOptions.Select((a, aIndex) => new AnswerOption
                {
                    Text = a.Text,
                    IsCorrect = a.IsCorrect,
                    Order = a.Order
                }).ToList()
            }).ToList();

            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<bool> ToggleQuizStatusAsync(int quizId)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null) return false;

            quiz.IsActive = !quiz.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        // Kategorije
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(int categoryId, Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(categoryId);
            if (existingCategory == null) return null;

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            await _context.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null) return false;

            // Provera da li postoje kvizovi u ovoj kategoriji
            var hasQuizzes = await _context.Quizzes.AnyAsync(q => q.CategoryId == categoryId);
            if (hasQuizzes)
            {
                throw new InvalidOperationException("Ne možete obrisati kategoriju koja ima kvizove.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        // Rezultati
        public async Task<List<QuizResult>> GetAllQuizResultsAsync()
        {
            return await _context.QuizResults
                .Include(qr => qr.User)
                .Include(qr => qr.Quiz)
                .OrderByDescending(qr => qr.CompletedAt)
                .Take(1000) // Limit za performanse
                .ToListAsync();
        }

        public async Task<List<QuizResult>> GetQuizResultsByQuizAsync(int quizId)
        {
            return await _context.QuizResults
                .Where(qr => qr.QuizId == quizId)
                .Include(qr => qr.User)
                .OrderByDescending(qr => qr.Score)
                .ThenBy(qr => qr.TimeSpent)
                .ToListAsync();
        }

        public async Task<List<QuizResult>> GetQuizResultsByUserAsync(int userId)
        {
            return await _context.QuizResults
                .Where(qr => qr.UserId == userId)
                .Include(qr => qr.Quiz)
                .OrderByDescending(qr => qr.CompletedAt)
                .ToListAsync();
        }

        // Statistika
        public async Task<AdminStatsDTO> GetAdminStatsAsync()
        {
            var stats = new AdminStatsDTO
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalQuizzes = await _context.Quizzes.CountAsync(),
                TotalQuizAttempts = await _context.QuizResults.CountAsync(),
                ActiveQuizzes = await _context.Quizzes.CountAsync(q => q.IsActive),
                AverageQuizScore = await _context.QuizResults.AverageAsync(qr => (double?)qr.Score) ?? 0
            };

            // Najpopularnija kategorija
            var popularCategory = await _context.Quizzes
                .GroupBy(q => q.Category.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            stats.MostPopularCategory = popularCategory ?? "Nema podataka";

            // Statistika po kategorijama
            var categoryStats = await _context.Quizzes
                .GroupBy(q => new { q.Category.Id, q.Category.Name })
                .Select(g => new CategoryStatsDTO
                {
                    CategoryName = g.Key.Name,
                    QuizCount = g.Count(),
                    AttemptCount = _context.QuizResults.Count(qr => qr.Quiz.CategoryId == g.Key.Id),
                    AverageScore = _context.QuizResults
                        .Where(qr => qr.Quiz.CategoryId == g.Key.Id)
                        .Average(qr => (double?)qr.Score) ?? 0
                })
                .ToListAsync();

            stats.CategoryStats = categoryStats;

            return stats;
        }
    }
}
