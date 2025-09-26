using KvizHub.Infrastructure.Services.Interfaces;
using KvizHub.Models.DTOs;
using KvizHub.Models.DTOs.KvizHub.Models.DTOs;
using KvizHub.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Infrastructure.Services.Implementations
{
	public class QuizService : IQuizService
	{
		private readonly ApplicationDbContext _context;

		public QuizService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<QuizListDTO>> GetAllQuizzesAsync()
		{
			return await _context.Quizzes
				.Where(q => q.IsActive)
				.Include(q => q.Category)
				.Include(q => q.Questions)
				.Select(q => new QuizListDTO
				{
					Id = q.Id,
					Title = q.Title,
					Description = q.Description,
					TimeLimit = q.TimeLimit,
					Difficulty = q.Difficulty,
					CategoryName = q.Category.Name,
					QuestionCount = q.Questions.Count
				})
				.ToListAsync();
		}

		public async Task<QuizDetailDTO> GetQuizByIdAsync(int id)
		{
			var quiz = await _context.Quizzes
				.Where(q => q.Id == id && q.IsActive)
				.Include(q => q.Category)
				.Include(q => q.Questions)
					.ThenInclude(qu => qu.AnswerOptions)
				.FirstOrDefaultAsync();

			if (quiz == null) return null;

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
								IsCorrect = ao.IsCorrect,
								Order = ao.Order
							}).ToList()
					}).ToList()
			};
		}

		public async Task<Quiz> CreateQuizAsync(CreateQuizDTO createQuizDto, int createdByUserId)
		{
			var quiz = new Quiz
			{
				Title = createQuizDto.Title,
				Description = createQuizDto.Description,
				TimeLimit = createQuizDto.TimeLimit,
				Difficulty = createQuizDto.Difficulty,
				CategoryId = createQuizDto.CategoryId,
				CreatedByUserId = createdByUserId,
				IsActive = true,
				Questions = createQuizDto.Questions.Select((q, index) => new Question
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

		public async Task<Quiz> UpdateQuizAsync(int id, CreateQuizDTO updateQuizDto)
		{
			var quiz = await _context.Quizzes
				.Include(q => q.Questions)
					.ThenInclude(qu => qu.AnswerOptions)
				.FirstOrDefaultAsync(q => q.Id == id);

			if (quiz == null) return null;

			// Ažuriraj osnovne podatke kviza
			quiz.Title = updateQuizDto.Title;
			quiz.Description = updateQuizDto.Description;
			quiz.TimeLimit = updateQuizDto.TimeLimit;
			quiz.Difficulty = updateQuizDto.Difficulty;
			quiz.CategoryId = updateQuizDto.CategoryId;

			// Obriši postojeća pitanja i odgovore
			_context.AnswerOptions.RemoveRange(quiz.Questions.SelectMany(q => q.AnswerOptions));
			_context.Questions.RemoveRange(quiz.Questions);

			// Dodaj nova pitanja i odgovore
			quiz.Questions = updateQuizDto.Questions.Select((q, index) => new Question
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

		public async Task<bool> DeleteQuizAsync(int id)
		{
			var quiz = await _context.Quizzes.FindAsync(id);
			if (quiz == null) return false;

			// Soft delete - samo postavimo IsActive na false
			quiz.IsActive = false;
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<List<QuizListDTO>> GetQuizzesByCategoryAsync(int categoryId)
		{
			return await _context.Quizzes
				.Where(q => q.IsActive && q.CategoryId == categoryId)
				.Include(q => q.Category)
				.Include(q => q.Questions)
				.Select(q => new QuizListDTO
				{
					Id = q.Id,
					Title = q.Title,
					Description = q.Description,
					TimeLimit = q.TimeLimit,
					Difficulty = q.Difficulty,
					CategoryName = q.Category.Name,
					QuestionCount = q.Questions.Count
				})
				.ToListAsync();
		}

		public async Task<List<QuizListDTO>> SearchQuizzesAsync(string searchTerm)
		{
			return await _context.Quizzes
				.Where(q => q.IsActive &&
						   (q.Title.Contains(searchTerm) ||
							q.Category.Name.Contains(searchTerm)))
				.Include(q => q.Category)
				.Include(q => q.Questions)
				.Select(q => new QuizListDTO
				{
					Id = q.Id,
					Title = q.Title,
					Description = q.Description,
					TimeLimit = q.TimeLimit,
					Difficulty = q.Difficulty,
					CategoryName = q.Category.Name,
					QuestionCount = q.Questions.Count
				})
				.ToListAsync();
		}
		public async Task<List<Category>> GetAllCategoriesAsync()
		{
			return await _context.Categories
				.OrderBy(c => c.Name)
				.ToListAsync();
		}
	}
}
