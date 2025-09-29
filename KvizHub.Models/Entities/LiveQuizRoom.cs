using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KvizHub.Models.Entities
{
    public class LiveQuizRoom
    {
        public int Id { get; set; }

        [Required]
        public string RoomCode { get; set; } = string.Empty; // Jedinstveni kod sobe (npr. "ABC123")

        [Required]
        public string RoomName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // KORISTIMO POSTOJEĆI QUIZ - ne pravimo novi!
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        // KORISTIMO POSTOJEĆEG USER-A za hosta
        public int HostUserId { get; set; }
        public User HostUser { get; set; } = null!;

        public int MaxParticipants { get; set; } = 50;
        public bool IsActive { get; set; } = true;
        public DateTime ScheduledStart { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Status sobe: Waiting, InProgress, Completed
        public string Status { get; set; } = "Waiting";

        // Trenutno pitanje (koristimo postojeći Question model)
        public int? CurrentQuestionId { get; set; }
        public Question? CurrentQuestion { get; set; }
        public DateTime? QuestionStartTime { get; set; }
        public int QuestionTimeLimit { get; set; } = 30; // sekundi po pitanju
    }
}
