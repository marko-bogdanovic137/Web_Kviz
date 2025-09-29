using Microsoft.AspNetCore.Mvc;
using KvizHub.Infrastructure.Services.Interfaces;
using KvizHub.Models.DTOs;
using KvizHub.Models.Entities;
using Web_KvizHub.Controllers;

namespace KvizHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // Korisnici
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                var users = await _adminService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Kvizovi
        [HttpGet("quizzes")]
        public async Task<IActionResult> GetAllQuizzes([FromQuery] bool includeInactive = false)
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                var quizzes = await _adminService.GetAllQuizzesAsync(includeInactive);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("quizzes")]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizDTO createQuizDto)
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                if (UserId == null)
                    return Unauthorized("Morate biti prijavljeni.");

                var quiz = await _adminService.CreateQuizAsync(createQuizDto, UserId.Value);
                return CreatedAtAction(nameof(GetAllQuizzes), quiz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("quizzes/{id}")]
        public async Task<IActionResult> UpdateQuiz(int id, [FromBody] CreateQuizDTO updateQuizDto)
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                var quiz = await _adminService.UpdateQuizAsync(id, updateQuizDto);
                if (quiz == null) return NotFound();
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("quizzes/{id}/toggle")]
        public async Task<IActionResult> ToggleQuizStatus(int id)
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                var result = await _adminService.ToggleQuizStatusAsync(id);
                if (!result) return NotFound();
                return Ok(new { message = "Status kviza uspešno promenjen." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Kategorije
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                var categories = await _adminService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                var createdCategory = await _adminService.CreateCategoryAsync(category);
                return Ok(createdCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                var updatedCategory = await _adminService.UpdateCategoryAsync(id, category);
                if (updatedCategory == null) return NotFound();
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                var result = await _adminService.DeleteCategoryAsync(id);
                if (!result) return NotFound();
                return Ok(new { message = "Kategorija uspešno obrisana." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Rezultati
        [HttpGet("results")]
        public async Task<IActionResult> GetAllResults()
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                var results = await _adminService.GetAllQuizResultsAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("results/quiz/{quizId}")]
        public async Task<IActionResult> GetResultsByQuiz(int quizId)
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                var results = await _adminService.GetQuizResultsByQuizAsync(quizId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("results/user/{userId}")]
        public async Task<IActionResult> GetResultsByUser(int userId)
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                var results = await _adminService.GetQuizResultsByUserAsync(userId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Statistika
        [HttpGet("stats")]
        public async Task<IActionResult> GetAdminStats()
        {
            try
            {
                if (!IsAdmin)
                    return Unauthorized("Samo administratori mogu da pristupe ovom resursu.");

                var stats = await _adminService.GetAdminStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}