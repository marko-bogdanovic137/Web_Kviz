using Microsoft.AspNetCore.Mvc;
using KvizHub.Infrastructure.Services.Interfaces;
using KvizHub.Models.DTOs;
using KvizHub.Models.Entities;

namespace Web_KvizHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : BaseController
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet("global")]
        public async Task<IActionResult> GetGlobalLeaderboard([FromQuery] string timeFrame = "all")
        {
            try
            {
                if (!IsValidTimeFrame(timeFrame))
                    return BadRequest("Nevalidan timeFrame. Dozvoljene vrednosti: all, weekly, monthly");

                var leaderboard = await _leaderboardService.GetGlobalLeaderboardAsync(timeFrame);
                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("quiz/{quizId}")]
        public async Task<IActionResult> GetQuizLeaderboard(int quizId, [FromQuery] string timeFrame = "all")
        {
            try
            {
                if (!IsValidTimeFrame(timeFrame))
                    return BadRequest("Nevalidan timeFrame. Dozvoljene vrednosti: all, weekly, monthly");

                var leaderboard = await _leaderboardService.GetQuizLeaderboardAsync(quizId, timeFrame);
                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-rank")]
        public async Task<IActionResult> GetMyRank([FromQuery] string timeFrame = "all")
        {
            try
            {
                if (UserId == null)
                    return Unauthorized("Morate biti prijavljeni da biste videli svoj rank.");

                if (!IsValidTimeFrame(timeFrame))
                    return BadRequest("Nevalidan timeFrame. Dozvoljene vrednosti: all, weekly, monthly");

                var userRank = await _leaderboardService.GetUserRankAsync(UserId.Value, timeFrame);
                return Ok(userRank);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private bool IsValidTimeFrame(string timeFrame)
        {
            return timeFrame.ToLower() switch
            {
                "all" or "weekly" or "monthly" => true,
                _ => false
            };
        }
    }
}