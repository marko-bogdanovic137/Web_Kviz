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
