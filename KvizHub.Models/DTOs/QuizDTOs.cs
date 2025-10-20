using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public class QuizDetailDTO
		{
			public int Id { get; set; }
			public string Title { get; set; } = string.Empty;
			public string Description { get; set; } = string.Empty;
			public int TimeLimit { get; set; }
			public string Difficulty { get; set; } = string.Empty;
			public string CategoryName { get; set; } = string.Empty;
			public int QuestionCount { get; set; }
			public List<QuestionDetailDTO> Questions { get; set; } = new();
		}

		public class QuestionDetailDTO
		{
			public int Id { get; set; }
			public string Text { get; set; } = string.Empty;
			public string Type { get; set; } = string.Empty;
			public int Points { get; set; }
			public int Order { get; set; }
			public List<AnswerOptionDTO> AnswerOptions { get; set; } = new();
		}

		public class AnswerOptionDTO
		{
			public int Id { get; set; }
			public string Text { get; set; } = string.Empty;
			public bool IsCorrect { get; set; }
			public int Order { get; set; }
		}
        // Rang lista DTO-ovi
        public class LeaderboardEntryDTO
        {
            public int Rank { get; set; }
            public int UserId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string? ProfileImage { get; set; }
            public int TotalScore { get; set; }
            public int QuizzesCompleted { get; set; }
            public double AveragePercentage { get; set; }
            public DateTime LastActivity { get; set; }
        }

        public class UserRankDTO
        {
            public int UserId { get; set; }
            public string Username { get; set; } = string.Empty;
            public int GlobalRank { get; set; }
            public int TotalUsers { get; set; }
            public int TotalScore { get; set; }
            public string TopCategory { get; set; } = string.Empty;
	    }
    public class QuizAdminDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TimeLimit { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CreatedByUsername { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    // Dodaj u QuizDTOs.cs
    public class CreateLiveQuizDTO
    {
        public string RoomName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int QuizId { get; set; }
        public int MaxParticipants { get; set; } = 50;
        public DateTime ScheduledStart { get; set; }
        public int QuestionTimeLimit { get; set; } = 30; // sekundi
    }

    public class LiveQuizResult
    {
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
        public int BasePoints { get; set; }
        public int SpeedBonus { get; set; }
        public double ResponseTimeSeconds { get; set; }
    }

    public class LiveQuizSummary
    {
        public string RoomCode { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int CurrentQuestion { get; set; }
        public List<LiveQuizParticipantDTO> Participants { get; set; } = new();
    }

    public class LiveQuizParticipantDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public int Score { get; set; }
        public int CorrectAnswers { get; set; }
        public bool IsConnected { get; set; }
    }

	public class QuizSubmissionDTO
	{
		public int QuizId { get; set; }
		public List<QuestionAnswerDTO> Answers { get; set; } = new();
		public int TimeSpent { get; set; } // u sekundama
	}

	public class QuestionAnswerDTO
	{
		public int QuestionId { get; set; }
		public List<int> SelectedAnswerIds { get; set; } = new(); // Za multiple choice
		public string? AnswerText { get; set; } // Za fill-in-the-blank
	}

	public class UserAnswerDTO
	{
		public string QuestionText { get; set; } = string.Empty;
		public string QuestionType { get; set; } = string.Empty;
		public List<string> SelectedAnswers { get; set; } = new();
		public List<string> CorrectAnswers { get; set; } = new();
		public bool IsCorrect { get; set; }
		public int PointsEarned { get; set; }
	}

	public class QuizResultDTO
	{
		public int Id { get; set; }
		public string UserName { get; set; } = string.Empty;
		public int QuizId { get; set; }
		public string QuizTitle { get; set; } = string.Empty;
		public int Score { get; set; }
		public int TotalQuestions { get; set; }
		public double Percentage { get; set; }
		public int TimeSpent { get; set; }
		public DateTime CompletedAt { get; set; }
		public List<UserAnswerDTO> UserAnswers { get; set; } = new();
	}
}
