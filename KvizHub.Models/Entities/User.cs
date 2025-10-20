using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KvizHub.Models.Entities
{
	public class User
	{
		public int Id { get; set; }

		[Required, MaxLength(50)]
		public string Username { get; set; } = string.Empty;

		[Required, EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		public string PasswordHash { get; set; } = string.Empty;

		public string? ProfileImage { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public bool IsAdmin { get; set; } = false;

		// Navigacija
		[JsonIgnore]
		public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
		[JsonIgnore]
		public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();
	}

}
