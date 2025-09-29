using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Models.Entities
{
    public class LiveQuizParticipant
    {
        public int Id { get; set; }

        // KORISTIMO POSTOJEĆEG USER-A
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // KORISTIMO POSTOJEĆU LIVE QUIZ SOBU
        public int LiveQuizRoomId { get; set; }
        public LiveQuizRoom LiveQuizRoom { get; set; } = null!;

        public string ConnectionId { get; set; } = string.Empty; // SignalR connection ID
        public int Score { get; set; }
        public int CorrectAnswers { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsConnected { get; set; } = true;
    }
}