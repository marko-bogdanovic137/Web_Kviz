using Microsoft.AspNetCore.Mvc;
using KvizHub.Infrastructure.Services.Interfaces;
using KvizHub.Models.DTOs;
using Web_KvizHub.Controllers;

namespace KvizHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizSolvingController : BaseController
    {
        private readonly IQuizSolvingService _quizSolvingService;

        public QuizSolvingController(IQuizSolvingService quizSolvingService)
        {
            _quizSolvingService = quizSolvingService;
        }

        [HttpGet("start/{quizId}")]
        public async Task<IActionResult> StartQuiz(int quizId)
        {
            try
            {
                var quiz = await _quizSolvingService.StartQuizAsync(quizId);
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmissionDTO submission)
        {
            try
            {
                // PROVERA: Da li je korisnik ulogovan?
                if (UserId == null)
                    return Unauthorized("Morate biti prijavljeni da biste završili kviz.");

                // KORISTI UserId iz BaseController-a (iz JWT tokena)
                var result = await _quizSolvingService.SubmitQuizAsync(
                    submission.QuizId, UserId.Value, submission);

                return Ok(new
                {
                    message = "Kviz uspešno završen!",
                    resultId = result.Id,
                    score = result.Score,
                    totalQuestions = result.TotalQuestions,
                    percentage = result.Percentage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("result/{resultId}")]
        public async Task<IActionResult> GetQuizResult(int resultId)
        {
            try
            {
                var result = await _quizSolvingService.GetQuizResultAsync(resultId);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetUserQuizHistory()
        {
            try
            {
                // PROVERA: Da li je korisnik ulogovan?
                if (UserId == null)
                    return Unauthorized("Morate biti prijavljeni da biste videli istoriju.");

                // KORISTI UserId iz BaseController-a (iz JWT tokena)
                var history = await _quizSolvingService.GetUserQuizHistoryAsync(UserId.Value);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}