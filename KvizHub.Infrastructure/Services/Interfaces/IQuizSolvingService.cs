using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KvizHub.Models.DTOs;
using KvizHub.Models.Entities;

namespace KvizHub.Infrastructure.Services.Interfaces
{
    public interface IQuizSolvingService
    {
        Task<QuizDetailDTO> StartQuizAsync(int quizId);
        Task<QuizResult> SubmitQuizAsync(int quizId, int userId, QuizSubmissionDTO submission);
        Task<QuizResultDTO> GetQuizResultAsync(int resultId);
        Task<List<QuizResultDTO>> GetUserQuizHistoryAsync(int userId);
    }
}

// Dodaj u QuizDTOs.cs
public class QuizSubmissionDTO
{
    public int QuizId { get; set; }
    public List<QuestionAnswerDTO> Answers { get; set; } = new();
    public int TimeSpent { get; set; } // u sekundama
}

public class QuestionAnswerDTO
{
    public int QuestionId { get; set; }
    public List<int> SelectedAnswerIds { get; set; } = new(); // Za multiple choice
    public string? AnswerText { get; set; } // Za fill-in-the-blank
}

public class QuizResultDTO
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public double Percentage { get; set; }
    public int TimeSpent { get; set; }
    public DateTime CompletedAt { get; set; }
    public List<UserAnswerDTO> UserAnswers { get; set; } = new();
}

public class UserAnswerDTO
{
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<string> SelectedAnswers { get; set; } = new();
    public List<string> CorrectAnswers { get; set; } = new();
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
}
