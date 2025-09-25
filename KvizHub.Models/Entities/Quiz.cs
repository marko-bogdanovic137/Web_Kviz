using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Models.Entities
{
	public class Quiz
	{
		public int Id { get; set; }
		[Required] public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public int TimeLimit { get; set; } // minuta
		public string Difficulty { get; set; } = "Medium";
		public bool IsActive { get; set; } = true;

		// FK
		public int CategoryId { get; set; }
		public int CreatedByUserId { get; set; }

		// Navigacija
		public Category Category { get; set; } = null!;
		public User CreatedByUser { get; set; } = null!;
		public ICollection<Question> Questions { get; set; } = new List<Question>();
		public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
	}

}
