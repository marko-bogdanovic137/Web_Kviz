using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KvizHub.Models.DTOs;

namespace KvizHub.Infrastructure.Services.Interfaces
{
    public interface ILeaderboardService
    {
        Task<List<LeaderboardEntryDTO>> GetGlobalLeaderboardAsync(string timeFrame = "all"); // all, weekly, monthly
        Task<List<LeaderboardEntryDTO>> GetQuizLeaderboardAsync(int quizId, string timeFrame = "all");
        Task<UserRankDTO> GetUserRankAsync(int userId, string timeFrame = "all");
    }
}
