using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KvizHub.Models.Entities
{
	public class QuizResult
	{
		public int Id { get; set; }
		public int Score { get; set; }
		public int TotalQuestions { get; set; }
		public double Percentage { get; set; }
		public int TimeSpent { get; set; } // sekunde
		public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

		public int UserId { get; set; }
		public int QuizId { get; set; }

		[JsonIgnore]
		public User User { get; set; } = null!;
		[JsonIgnore]
		public Quiz Quiz { get; set; } = null!;

		// UserAnswers sada čuvamo **bez FK ka Question**, samo vezu ka QuizResult
		public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
	}

}
