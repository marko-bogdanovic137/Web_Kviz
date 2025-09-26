using KvizHub.Infrastructure.Services.Interfaces;
using KvizHub.Models.DTOs;
using KvizHub.Models.DTOs.KvizHub.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Web_KvizHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerDto)
		{
			try
			{
				var result = await _authService.RegisterAsync(registerDto);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDto)
		{
			try
			{
				var result = await _authService.LoginAsync(loginDto);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
		}
	}
}