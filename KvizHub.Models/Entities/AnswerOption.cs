using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Models.Entities
{
	public class AnswerOption
	{
		public int Id { get; set; }
		public string Text { get; set; } = string.Empty;
		public bool IsCorrect { get; set; }
		public int Order { get; set; }

		// Foreign keys
		public int QuestionId { get; set; }

		// Navigation properties
		public Question Question { get; set; } = null!;
	}
}
