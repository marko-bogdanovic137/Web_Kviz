using KvizHub.Models.DTOs;
using KvizHub.Models.DTOs.KvizHub.Models.DTOs;
using KvizHub.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Infrastructure.Services.Interfaces
{
	public interface IQuizService
	{
		Task<List<QuizListDTO>> GetAllQuizzesAsync();
		Task<QuizDetailDTO> GetQuizByIdAsync(int id);
		Task<Quiz> CreateQuizAsync(CreateQuizDTO createQuizDto, int createdByUserId);
		Task<Quiz> UpdateQuizAsync(int id, CreateQuizDTO updateQuizDto);
		Task<bool> DeleteQuizAsync(int id);
		Task<List<QuizListDTO>> GetQuizzesByCategoryAsync(int categoryId);
		Task<List<QuizListDTO>> SearchQuizzesAsync(string searchTerm);
		Task<List<Category>> GetAllCategoriesAsync();
	}
}
