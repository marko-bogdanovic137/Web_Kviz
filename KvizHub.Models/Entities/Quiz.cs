using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Models.Entities
{
	public class Quiz
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public int TimeLimit { get; set; } // u minutima
		public string Difficulty { get; set; } = "Medium"; // Easy, Medium, Hard
		public bool IsActive { get; set; } = true;

		// Foreign keys
		public int CategoryId { get; set; }
		public int CreatedByUserId { get; set; }

		// Navigation properties
		public Category Category { get; set; } = null!;
		public User CreatedByUser { get; set; } = null!;
		public ICollection<Question> Questions { get; set; } = new List<Question>();
		public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
	}
}
