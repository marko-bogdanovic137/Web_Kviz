using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Models.Entities
{
	public class SelectedAnswer
	{
		public int Id { get; set; }

		// Foreign keys
		public int UserAnswerId { get; set; }
		public int AnswerOptionId { get; set; }

		// Navigation properties
		public UserAnswer UserAnswer { get; set; } = null!;
		public AnswerOption AnswerOption { get; set; } = null!;
	}
}
