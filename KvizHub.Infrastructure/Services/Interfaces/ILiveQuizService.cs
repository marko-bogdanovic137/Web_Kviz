using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KvizHub.Models.DTOs;
using KvizHub.Models.Entities;

namespace KvizHub.Infrastructure.Services.Interfaces
{
    public interface ILiveQuizService
    {
        // Upravljanje sobama
        Task<LiveQuizRoom> CreateRoomAsync(CreateLiveQuizDTO createDto, int hostUserId);
        Task<LiveQuizRoom> GetRoomByCodeAsync(string roomCode);
        Task<List<LiveQuizRoom>> GetActiveRoomsAsync();
        Task<bool> CloseRoomAsync(string roomCode, int hostUserId);

        // Učesnici
        Task<LiveQuizParticipant> JoinRoomAsync(string roomCode, int userId, string connectionId);
        Task<bool> LeaveRoomAsync(string roomCode, string connectionId);
        Task<List<LiveQuizParticipant>> GetRoomParticipantsAsync(string roomCode);
        Task<List<LiveQuizParticipant>> GetRoomParticipantsByConnectionIdAsync(string connectionId);

        // Kviz logika
        Task StartQuizAsync(string roomCode, int hostUserId);
        Task StartQuestionAsync(string roomCode, int questionId, int hostUserId);
        Task EndQuestionAsync(string roomCode, int hostUserId);
        Task<LiveQuizResult> SubmitAnswerAsync(string roomCode, string connectionId, int questionId,
            List<int> selectedAnswerIds, string answerText);

        // Rezultati
        Task<LiveQuizSummary> GetQuizSummaryAsync(string roomCode);
        Task<List<LiveQuizParticipant>> GetLeaderboardAsync(string roomCode);
    }
}
