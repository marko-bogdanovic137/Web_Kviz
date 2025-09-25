using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KvizHub.Models.Entities
{
	public class UserAnswer
	{
		public int Id { get; set; }
		public string? AnswerText { get; set; } // fill-in-the-blank
		public bool IsCorrect { get; set; }

		// FK
		public int QuizResultId { get; set; }

		public QuizResult QuizResult { get; set; } = null!;

		// SelectedAnswers čuvaju samo vezu ka AnswerOption
		public ICollection<SelectedAnswer> SelectedAnswers { get; set; } = new List<SelectedAnswer>();
	}

}