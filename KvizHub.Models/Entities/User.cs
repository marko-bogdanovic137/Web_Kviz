using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

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

		// Navigacija
		public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
		public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();
	}

}
