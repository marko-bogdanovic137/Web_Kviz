using Microsoft.AspNetCore.SignalR;
using KvizHub.Models.DTOs;
using KvizHub.Infrastructure.Services.Interfaces;

namespace KvizHub.Infrastructure.Hubs
{
    public class LiveQuizHub : Hub
    {
        private readonly ILiveQuizService _liveQuizService;

        public LiveQuizHub(ILiveQuizService liveQuizService)
        {
            _liveQuizService = liveQuizService;
        }

        // Pridruživanje sobi - SADA SA PRAVOM LOGIKOM
        public async Task JoinRoom(string roomCode, int userId, string username)
        {
            try
            {
                // Koristi service za pridruživanje
                var participant = await _liveQuizService.JoinRoomAsync(roomCode, userId, Context.ConnectionId);

                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

                // Objavi svima da se novi korisnik pridružio
                await Clients.Group(roomCode).SendAsync("UserJoined", new
                {
                    UserId = userId,
                    Username = username,
                    ConnectionId = Context.ConnectionId,
                    Score = participant.Score
                });

                // Pošalji ažuriranu listu učesnika
                var participants = await _liveQuizService.GetRoomParticipantsAsync(roomCode);
                await Clients.Group(roomCode).SendAsync("ParticipantsUpdated", participants);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        // Slanje odgovora - SADA SA PRAVOM LOGIKOM
        public async Task SubmitAnswer(string roomCode, int questionId, List<int> selectedAnswerIds, string answerText)
        {
            try
            {
                var result = await _liveQuizService.SubmitAnswerAsync(
                    roomCode, Context.ConnectionId, questionId, selectedAnswerIds, answerText);

                // Objavi rezultat svima u sobi
                await Clients.Group(roomCode).SendAsync("AnswerSubmitted", new
                {
                    UserConnectionId = Context.ConnectionId,
                    QuestionId = questionId,
                    IsCorrect = result.IsCorrect,
                    PointsEarned = result.PointsEarned,
                    BasePoints = result.BasePoints,
                    SpeedBonus = result.SpeedBonus,
                    ResponseTimeSeconds = result.ResponseTimeSeconds,
                    AnsweredAt = DateTime.UtcNow
                });

                // Ažuriraj rang listu
                var leaderboard = await _liveQuizService.GetLeaderboardAsync(roomCode);
                await Clients.Group(roomCode).SendAsync("LeaderboardUpdated", leaderboard);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        // Kada korisnik napusti sobu
        public async Task LeaveRoom(string roomCode)
        {
            try
            {
                await _liveQuizService.LeaveRoomAsync(roomCode, Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);

                var participants = await _liveQuizService.GetRoomParticipantsAsync(roomCode);
                await Clients.Group(roomCode).SendAsync("ParticipantsUpdated", participants);
                await Clients.Group(roomCode).SendAsync("UserLeft", Context.ConnectionId);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        // Konekcija i diskonekcija
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Automatski napusti sve sobe pri diskonekciji
            var participants = await _liveQuizService.GetRoomParticipantsByConnectionIdAsync(Context.ConnectionId);
            foreach (var participant in participants)
            {
                await LeaveRoom(participant.LiveQuizRoom.RoomCode);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}