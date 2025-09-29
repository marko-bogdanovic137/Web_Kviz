using KvizHub.Core.Services.Interfaces;
using KvizHub.Models.DTOs;
using KvizHub.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using KvizHub.Infrastructure.Services.Interfaces;

namespace KvizHub.Infrastructure.Services.Implementations
{
	public class AuthService : IAuthService
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;

		public AuthService(ApplicationDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		public async Task<bool> UserExistsAsync(string username, string email)
		{
			return await _context.Users
				.AnyAsync(u => u.Username == username || u.Email == email);
		}

		public async Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO registerDto)
		{
			// Provera da li korisnik već postoji
			if (await UserExistsAsync(registerDto.Username, registerDto.Email))
			{
				throw new Exception("Korisnik sa ovim username-om ili email-om već postoji.");
			}

			// Kreiranje novog korisnika
			var user = new User
			{
				Username = registerDto.Username,
				Email = registerDto.Email,
				ProfileImage = registerDto.ProfileImage,
				CreatedAt = DateTime.UtcNow
			};

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			// Generisanje JWT tokena
			var token = GenerateJwtToken(user);

			return new LoginResponseDTO
			{
				Id = user.Id,
				Username = user.Username,
				Email = user.Email,
				ProfileImage = user.ProfileImage,
				Token = token
			};
		}

		public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginDto)
		{
			// Pronalaženje korisnika po username-u ili email-u
			var user = await _context.Users
				.FirstOrDefaultAsync(u => u.Username == loginDto.UsernameOrEmail ||
										 u.Email == loginDto.UsernameOrEmail);

			if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
			{
				throw new Exception("Pogrešno korisničko ime/email ili lozinka.");
			}

			// Generisanje JWT tokena
			var token = GenerateJwtToken(user);

			return new LoginResponseDTO
			{
				Id = user.Id,
				Username = user.Username,
				Email = user.Email,
				ProfileImage = user.ProfileImage,
				Token = token
			};
		}

		private string GenerateJwtToken(User user)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.Email, user.Email)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
				_configuration["Jwt:Key"] ?? "default-key-minimum-16-chars"));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddHours(2),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
