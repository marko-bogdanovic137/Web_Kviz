using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KvizHub.Models.DTOs;
using KvizHub.Models.Entities;

namespace KvizHub.Infrastructure.Services.Interfaces
{
    public interface IAdminService
    {
        // Korisnici
        Task<List<UserDTO>> GetAllUsersAsync();
        //Task<bool> ToggleUserStatusAsync(int userId);
        Task<bool> DeleteUserAsync(int userId);

		// Kvizovi - admin može da vidi i neaktivne
		Task<List<QuizAdminDTO>> GetAllQuizzesAsync(bool includeInactive = false);
        Task<Quiz> CreateQuizAsync(CreateQuizDTO createQuizDto, int adminUserId);
        Task<Quiz> UpdateQuizAsync(int quizId, CreateQuizDTO updateQuizDto);
        Task<bool> ToggleQuizStatusAsync(int quizId);

        Task<bool> DeleteQuizAsync(int quizId);

		// Kategorije
		Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(int categoryId, Category category);
        Task<bool> DeleteCategoryAsync(int categoryId);

        // Rezultati
        Task<List<QuizResult>> GetAllQuizResultsAsync();
        Task<List<QuizResult>> GetQuizResultsByQuizAsync(int quizId);
        Task<List<QuizResult>> GetQuizResultsByUserAsync(int userId);

		// Statistika
		Task<AdminStatsDTO> GetAdminStatsAsync();
    }
}

public class AdminStatsDTO
{
    public int TotalUsers { get; set; }
    public int TotalQuizzes { get; set; }
    public int TotalQuizAttempts { get; set; }
    public int ActiveQuizzes { get; set; }
    public double AverageQuizScore { get; set; }
    public string MostPopularCategory { get; set; } = string.Empty;
    public List<CategoryStatsDTO> CategoryStats { get; set; } = new();
}

public class CategoryStatsDTO
{
    public string CategoryName { get; set; } = string.Empty;
    public int QuizCount { get; set; }
    public int AttemptCount { get; set; }
    public double AverageScore { get; set; }
}
