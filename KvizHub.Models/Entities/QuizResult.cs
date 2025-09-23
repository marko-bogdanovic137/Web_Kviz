using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Models.Entities
{
	public class QuizResult
	{
		public int Id { get; set; }
		public int Score { get; set; }
		public int TotalQuestions { get; set; }
		public double Percentage { get; set; }
		public int TimeSpent { get; set; } // u sekundama
		public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

		// Foreign keys
		public int UserId { get; set; }
		public int QuizId { get; set; }

		// Navigation properties
		public User User { get; set; } = null!;
		public Quiz Quiz { get; set; } = null!;
		public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
	}
}
