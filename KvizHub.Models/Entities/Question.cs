using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KvizHub.Models.Entities
{
	public class Question
	{
		public int Id { get; set; }
		[Required] public string Text { get; set; } = string.Empty;
		public string Type { get; set; } = "MultipleChoice";
		public int Points { get; set; } = 1;
		public int Order { get; set; }

		public int QuizId { get; set; }

		[JsonIgnore]
		public Quiz Quiz { get; set; } = null!;
		public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
	}

}
