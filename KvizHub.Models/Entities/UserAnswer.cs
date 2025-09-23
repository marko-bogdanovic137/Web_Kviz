using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Models.Entities
{
	public class UserAnswer
	{
		public int Id { get; set; }
		public string? AnswerText { get; set; } // Za fill-in-the-blank pitanja
		public bool IsCorrect { get; set; }

		// Foreign keys
		public int QuizResultId { get; set; }
		public int QuestionId { get; set; }

		// Navigation properties
		public QuizResult QuizResult { get; set; } = null!;
		public Question Question { get; set; } = null!;
		public ICollection<SelectedAnswer> SelectedAnswers { get; set; } = new List<SelectedAnswer>();
	}
}
