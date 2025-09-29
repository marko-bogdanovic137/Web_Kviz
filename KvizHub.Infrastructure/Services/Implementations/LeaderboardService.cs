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
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ApplicationDbContext _context;

        public LeaderboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LeaderboardEntryDTO>> GetGlobalLeaderboardAsync(string timeFrame = "all")
        {
            var timeFilter = GetTimeFilter(timeFrame);

            var leaderboard = await _context.QuizResults
                .Where(qr => timeFilter == null || qr.CompletedAt >= timeFilter)
                .GroupBy(qr => new { qr.UserId, qr.User.Username, qr.User.ProfileImage })
                .Select(g => new LeaderboardEntryDTO
                {
                    UserId = g.Key.UserId,
                    Username = g.Key.Username,
                    ProfileImage = g.Key.ProfileImage,
                    TotalScore = g.Sum(qr => qr.Score),
                    QuizzesCompleted = g.Count(),
                    AveragePercentage = g.Average(qr => qr.Percentage),
                    LastActivity = g.Max(qr => qr.CompletedAt)
                })
                .OrderByDescending(e => e.TotalScore)
                .ThenByDescending(e => e.AveragePercentage)
                .Take(100) // Top 100 korisnika
                .ToListAsync();

            // Dodaj rangove
            for (int i = 0; i < leaderboard.Count; i++)
            {
                leaderboard[i].Rank = i + 1;
            }

            return leaderboard;
        }

        public async Task<List<LeaderboardEntryDTO>> GetQuizLeaderboardAsync(int quizId, string timeFrame = "all")
        {
            var timeFilter = GetTimeFilter(timeFrame);

            var leaderboard = await _context.QuizResults
                .Where(qr => qr.QuizId == quizId && (timeFilter == null || qr.CompletedAt >= timeFilter))
                .GroupBy(qr => new { qr.UserId, qr.User.Username, qr.User.ProfileImage })
                .Select(g => new LeaderboardEntryDTO
                {
                    UserId = g.Key.UserId,
                    Username = g.Key.Username,
                    ProfileImage = g.Key.ProfileImage,
                    TotalScore = g.Max(qr => qr.Score), // Najbolji rezultat za ovaj kviz
                    QuizzesCompleted = g.Count(),
                    AveragePercentage = g.Average(qr => qr.Percentage),
                    LastActivity = g.Max(qr => qr.CompletedAt)
                })
                .OrderByDescending(e => e.TotalScore)
                .ThenBy(e => e.LastActivity) // Ako je isti score, ranije završeni je bolji
                .Take(50) // Top 50 za kviz
                .ToListAsync();

            // Dodaj rangove
            for (int i = 0; i < leaderboard.Count; i++)
            {
                leaderboard[i].Rank = i + 1;
            }

            return leaderboard;
        }

        public async Task<UserRankDTO> GetUserRankAsync(int userId, string timeFrame = "all")
        {
            var timeFilter = GetTimeFilter(timeFrame);

            // Globalni rank
            var userTotalScore = await _context.QuizResults
                .Where(qr => qr.UserId == userId && (timeFilter == null || qr.CompletedAt >= timeFilter))
                .SumAsync(qr => qr.Score);

            var usersWithHigherScore = await _context.QuizResults
                .Where(qr => timeFilter == null || qr.CompletedAt >= timeFilter)
                .GroupBy(qr => qr.UserId)
                .Where(g => g.Sum(qr => qr.Score) > userTotalScore)
                .CountAsync();

            var totalUsers = await _context.QuizResults
                .Where(qr => timeFilter == null || qr.CompletedAt >= timeFilter)
                .Select(qr => qr.UserId)
                .Distinct()
                .CountAsync();

            // Najpopularnija kategorija
            var topCategory = await _context.QuizResults
                .Where(qr => qr.UserId == userId && (timeFilter == null || qr.CompletedAt >= timeFilter))
                .GroupBy(qr => qr.Quiz.Category.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync() ?? "Nema podataka";

            return new UserRankDTO
            {
                UserId = userId,
                Username = await _context.Users.Where(u => u.Id == userId).Select(u => u.Username).FirstAsync(),
                GlobalRank = usersWithHigherScore + 1,
                TotalUsers = totalUsers,
                TotalScore = userTotalScore,
                TopCategory = topCategory
            };
        }

        private DateTime? GetTimeFilter(string timeFrame)
        {
            return timeFrame.ToLower() switch
            {
                "weekly" => DateTime.UtcNow.AddDays(-7),
                "monthly" => DateTime.UtcNow.AddDays(-30),
                _ => null // all time
            };
        }
    }
}