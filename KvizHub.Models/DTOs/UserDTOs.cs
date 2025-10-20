using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

	namespace KvizHub.Models.DTOs
	{
		// DTO za registraciju
		public class RegisterRequestDTO
		{
			public string Username { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
			public string Password { get; set; } = string.Empty;
			public string? ProfileImage { get; set; }
		}

		// DTO za login
		public class LoginRequestDTO
		{
			public string UsernameOrEmail { get; set; } = string.Empty;
			public string Password { get; set; } = string.Empty;
		}

		// DTO koji šaljemo na frontend nakon uspešnog logina
		public class LoginResponseDTO
		{
			public int Id { get; set; }
			public string Username { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
			public string? ProfileImage { get; set; }
			public string Token { get; set; } = string.Empty;
			public bool IsAdmin { get; set; }
		}

		// DTO za prikaz korisnika (bez osetljivih podataka)
		public class UserDTO
		{
			public int Id { get; set; }
			public string Username { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
			public string? ProfileImage { get; set; }
			public DateTime CreatedAt { get; set; }
			public bool IsAdmin { get; set; }
		}
		public class UserUpdateDTO
		{
			public string? Username { get; set; }
			public string? Email { get; set; }
			public string? ProfileImage { get; set; }
		}
	}
