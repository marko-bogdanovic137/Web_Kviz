using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Models.Entities
{
	public class Question
	{
		public int Id { get; set; }
		public string Text { get; set; } = string.Empty;
		public string Type { get; set; } = "MultipleChoice"; // MultipleChoice, MultipleAnswer, TrueFalse, FillInBlank
		public int Points { get; set; } = 1;
		public int Order { get; set; } // Redosled pitanja u kvizu

		// Foreign keys
		public int QuizId { get; set; }

		// Navigation properties
		public Quiz Quiz { get; set; } = null!;
		public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
	}
}
