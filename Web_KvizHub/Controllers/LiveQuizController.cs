using Microsoft.AspNetCore.Mvc;
using KvizHub.Infrastructure.Services.Interfaces;
using KvizHub.Models.DTOs;
using KvizHub.Models.Entities;

namespace Web_KvizHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LiveQuizController : BaseController
    {
        private readonly ILiveQuizService _liveQuizService;

        public LiveQuizController(ILiveQuizService liveQuizService)
        {
            _liveQuizService = liveQuizService;
        }

        [HttpGet("rooms")]
        public async Task<IActionResult> GetActiveRooms()
        {
            try
            {
                var rooms = await _liveQuizService.GetActiveRoomsAsync();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("rooms/{roomCode}")]
        public async Task<IActionResult> GetRoom(string roomCode)
        {
            try
            {
                var room = await _liveQuizService.GetRoomByCodeAsync(roomCode);
                if (room == null) return NotFound();
                return Ok(room);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateLiveQuizDTO createDto)
        {
            try
            {
                if (UserId == null)
                    return Unauthorized("Morate biti prijavljeni.");

                var room = await _liveQuizService.CreateRoomAsync(createDto, UserId.Value);
                return Ok(room);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("rooms/{roomCode}/join")]
        public async Task<IActionResult> JoinRoom(string roomCode)
        {
            try
            {
                if (UserId == null)
                    return Unauthorized("Morate biti prijavljeni.");

                // ConnectionId će biti postavljen preko SignalR hub-a
                var participant = await _liveQuizService.JoinRoomAsync(roomCode, UserId.Value, "temp-connection");
                return Ok(new { message = "Uspešno ste se pridružili sobi", participant });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("rooms/{roomCode}/participants")]
        public async Task<IActionResult> GetRoomParticipants(string roomCode)
        {
            try
            {
                var participants = await _liveQuizService.GetRoomParticipantsAsync(roomCode);
                return Ok(participants);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("rooms/{roomCode}/start")]
        public async Task<IActionResult> StartQuiz(string roomCode)
        {
            try
            {
                if (UserId == null)
                    return Unauthorized("Morate biti prijavljeni.");

                await _liveQuizService.StartQuizAsync(roomCode, UserId.Value);
                return Ok(new { message = "Kviz je pokrenut" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("rooms/{roomCode}/questions/{questionId}/start")]
        public async Task<IActionResult> StartQuestion(string roomCode, int questionId)
        {
            try
            {
                if (UserId == null)
                    return Unauthorized("Morate biti prijavljeni.");

                await _liveQuizService.StartQuestionAsync(roomCode, questionId, UserId.Value);
                return Ok(new { message = "Pitanje je pokrenuto" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("rooms/{roomCode}/questions/end")]
        public async Task<IActionResult> EndQuestion(string roomCode)
        {
            try
            {
                if (UserId == null)
                    return Unauthorized("Morate biti prijavljeni.");

                await _liveQuizService.EndQuestionAsync(roomCode, UserId.Value);
                return Ok(new { message = "Pitanje je završeno" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("rooms/{roomCode}/leaderboard")]
        public async Task<IActionResult> GetLeaderboard(string roomCode)
        {
            try
            {
                var leaderboard = await _liveQuizService.GetLeaderboardAsync(roomCode);
                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}