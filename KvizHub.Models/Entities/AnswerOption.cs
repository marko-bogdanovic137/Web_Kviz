using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KvizHub.Models.Entities
{
	public class AnswerOption
	{
		public int Id { get; set; }
		[Required] public string Text { get; set; } = string.Empty;
		public bool IsCorrect { get; set; }
		public int Order { get; set; }

		public int QuestionId { get; set; }
		[JsonIgnore]
		public Question Question { get; set; } = null!;
	}

}
