using KvizHub.Infrastructure.Services.Interfaces;
using KvizHub.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Web_KvizHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class QuizzesController : ControllerBase
	{
		private readonly IQuizService _quizService;

		public QuizzesController(IQuizService quizService)
		{
			_quizService = quizService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllQuizzes()
		{
			var quizzes = await _quizService.GetAllQuizzesAsync();
			return Ok(quizzes);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetQuizById(int id)
		{
			var quiz = await _quizService.GetQuizByIdAsync(id);
			if (quiz == null) return NotFound();
			return Ok(quiz);
		}

		[HttpGet("category/{categoryId}")]
		public async Task<IActionResult> GetQuizzesByCategory(int categoryId)
		{
			var quizzes = await _quizService.GetQuizzesByCategoryAsync(categoryId);
			return Ok(quizzes);
		}

		[HttpGet("search")]
		public async Task<IActionResult> SearchQuizzes([FromQuery] string term)
		{
			if (string.IsNullOrWhiteSpace(term))
				return BadRequest("Search term is required");

			var quizzes = await _quizService.SearchQuizzesAsync(term);
			return Ok(quizzes);
		}

		// TODO: Dodati autorizaciju za admin metode
		[HttpPost]
		public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizDTO createQuizDto)
		{
			try
			{
				// TODO: Dohvatiti userId iz JWT tokena
				int createdByUserId = 1; // Privremeno - zamenićemo sa pravim userId

				var quiz = await _quizService.CreateQuizAsync(createQuizDto, createdByUserId);
				return CreatedAtAction(nameof(GetQuizById), new { id = quiz.Id }, quiz);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateQuiz(int id, [FromBody] CreateQuizDTO updateQuizDto)
		{
			try
			{
				var quiz = await _quizService.UpdateQuizAsync(id, updateQuizDto);
				if (quiz == null) return NotFound();
				return Ok(quiz);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteQuiz(int id)
		{
			var result = await _quizService.DeleteQuizAsync(id);
			if (!result) return NotFound();
			return NoContent();
		}

		[HttpGet("categories")]
		public async Task<IActionResult> GetAllCategories()
		{
			var categories = await _quizService.GetAllCategoriesAsync();
			return Ok(categories);
		}
	}
}