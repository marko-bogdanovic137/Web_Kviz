using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Models.DTOs
{
	namespace KvizHub.Models.DTOs
	{
		// DTO za prikaz kviza u listi
		public class QuizListDTO
		{
			public int Id { get; set; }
			public string Title { get; set; } = string.Empty;
			public string Description { get; set; } = string.Empty;
			public int TimeLimit { get; set; }
			public string Difficulty { get; set; } = string.Empty;
			public string CategoryName { get; set; } = string.Empty;
			public int QuestionCount { get; set; }
		}

		// DTO za kreiranje novog kviza (admin)
		public class CreateQuizDTO
		{
			public string Title { get; set; } = string.Empty;
			public string Description { get; set; } = string.Empty;
			public int TimeLimit { get; set; }
			public string Difficulty { get; set; } = string.Empty;
			public int CategoryId { get; set; }
			public List<CreateQuestionDTO> Questions { get; set; } = new();
		}

		public class CreateQuestionDTO
		{
			public string Text { get; set; } = string.Empty;
			public string Type { get; set; } = "MultipleChoice";
			public int Points { get; set; } = 1;
			public int Order { get; set; }
			public List<CreateAnswerOptionDTO> AnswerOptions { get; set; } = new();
		}

		public class CreateAnswerOptionDTO
		{
			public string Text { get; set; } = string.Empty;
			public bool IsCorrect { get; set; }
			public int Order { get; set; }
		}
	}
}
