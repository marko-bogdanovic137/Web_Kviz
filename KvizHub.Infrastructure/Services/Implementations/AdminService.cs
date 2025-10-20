using System;
using System.Collections.Generic;
using System.Linq;
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

		// ----------------------------------------
		// KORISNICI
		// ----------------------------------------
		public async Task<List<UserDTO>> GetAllUsersAsync()
		{
			return await _context.Users
				.AsNoTracking()
				.Select(u => new UserDTO
				{
					Id = u.Id,
					Username = u.Username,
					Email = u.Email,
					ProfileImage = u.ProfileImage,
					CreatedAt = u.CreatedAt,
					IsAdmin = u.IsAdmin
				})
				.OrderBy(u => u.CreatedAt)
				.ToListAsync();
		}

		public async Task<bool> DeleteUserAsync(int userId)
		{
			var user = await _context.Users.FindAsync(userId);
			if (user == null) return false;

			var userResults = await _context.QuizResults.Where(qr => qr.UserId == userId).ToListAsync();
			_context.QuizResults.RemoveRange(userResults);

			var userQuizzes = await _context.Quizzes.Where(q => q.CreatedByUserId == userId).ToListAsync();
			var adminUser = await _context.Users.FirstAsync(u => u.IsAdmin); // Nađi admina
			foreach (var quiz in userQuizzes)
			{
				quiz.CreatedByUserId = adminUser.Id;
			}

			_context.Users.Remove(user);
			await _context.SaveChangesAsync();
			return true;
		}

		// ----------------------------------------
		// KVIZOVI
		// ----------------------------------------
		public async Task<List<QuizAdminDTO>> GetAllQuizzesAsync(bool includeInactive = false)
		{
			var query = _context.Quizzes
				.Include(q => q.Category)
				.Include(q => q.CreatedByUser)
				.Include(q => q.Questions)
				.AsQueryable();

			return await query
				.AsNoTracking()
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
				Questions = createQuizDto.Questions.Select(q => new Question
				{
					Text = q.Text,
					Type = q.Type,
					Points = q.Points,
					Order = q.Order,
					AnswerOptions = q.AnswerOptions.Select(a => new AnswerOption
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

		public async Task<bool> DeleteQuizAsync(int quizId)
		{
			var quiz = await _context.Quizzes
				.Include(q => q.Questions)
					.ThenInclude(q => q.AnswerOptions)
				.Include(q => q.QuizResults)
				.FirstOrDefaultAsync(q => q.Id == quizId);

			if (quiz == null) return false;

			_context.QuizResults.RemoveRange(quiz.QuizResults);

			foreach (var question in quiz.Questions)
			{
				_context.AnswerOptions.RemoveRange(question.AnswerOptions);
			}
			_context.Questions.RemoveRange(quiz.Questions);

			_context.Quizzes.Remove(quiz);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ToggleQuizStatusAsync(int quizId)
		{
			var quiz = await _context.Quizzes.FindAsync(quizId);
			if (quiz == null) return false;

			quiz.IsActive = !quiz.IsActive;
			await _context.SaveChangesAsync();
			return true;
		}

		// ----------------------------------------
		// KATEGORIJE
		// ----------------------------------------
		public async Task<List<Category>> GetAllCategoriesAsync()
		{
			return await _context.Categories
				.OrderBy(c => c.Name)
				.AsNoTracking()
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
			var existing = await _context.Categories.FindAsync(categoryId);
			if (existing == null) return null;

			existing.Name = category.Name;
			existing.Description = category.Description;
			await _context.SaveChangesAsync();
			return existing;
		}

		public async Task<bool> DeleteCategoryAsync(int categoryId)
		{
			var category = await _context.Categories.FindAsync(categoryId);
			if (category == null) return false;

			var hasQuizzes = await _context.Quizzes.AnyAsync(q => q.CategoryId == categoryId);
			if (hasQuizzes)
				throw new InvalidOperationException("Ne možete obrisati kategoriju koja ima kvizove.");

			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();
			return true;
		}

		// ----------------------------------------
		// REZULTATI
		// ----------------------------------------
		public async Task<List<QuizResult>> GetAllQuizResultsAsync()
		{
			return await _context.QuizResults
				.Include(qr => qr.User)
				.Include(qr => qr.Quiz)
				.OrderByDescending(qr => qr.CompletedAt)
				.Take(1000)
				.AsNoTracking()
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

		// ----------------------------------------
		// STATISTIKA
		// ----------------------------------------
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

			var popularCategory = await _context.Quizzes
				.GroupBy(q => q.Category.Name)
				.OrderByDescending(g => g.Count())
				.Select(g => g.Key)
				.FirstOrDefaultAsync();

			stats.MostPopularCategory = popularCategory ?? "Nema podataka";

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

		public async Task<Quiz> UpdateQuizAsync(int quizId, CreateQuizDTO updateQuizDto)
		{
			var quiz = await _context.Quizzes.Include(q => q.Questions).ThenInclude(qu => qu.AnswerOptions).FirstOrDefaultAsync(q => q.Id == quizId); if (quiz == null) return null;
			quiz.Title = updateQuizDto.Title; quiz.Description = updateQuizDto.Description; quiz.TimeLimit = updateQuizDto.TimeLimit; quiz.Difficulty = updateQuizDto.Difficulty;
			quiz.CategoryId = updateQuizDto.CategoryId;
			_context.AnswerOptions.RemoveRange(quiz.Questions.SelectMany(q => q.AnswerOptions)); _context.Questions.RemoveRange(quiz.Questions);
			quiz.Questions = updateQuizDto.Questions.Select((q, qIndex) =>
			new Question
			{
				Text = q.Text,
				Type = q.Type,
				Points = q.Points,
				Order = q.Order,
				AnswerOptions = q.AnswerOptions.Select((a, aIndex) =>
			new AnswerOption { Text = a.Text, IsCorrect = a.IsCorrect, Order = a.Order }).ToList()
			}).ToList(); await _context.SaveChangesAsync(); return quiz;
		}
	}
}
