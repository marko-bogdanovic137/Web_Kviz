using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KvizHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLiveQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiveQuizRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    HostUserId = table.Column<int>(type: "int", nullable: false),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ScheduledStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentQuestionId = table.Column<int>(type: "int", nullable: true),
                    QuestionStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuestionTimeLimit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveQuizRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveQuizRooms_Questions_CurrentQuestionId",
                        column: x => x.CurrentQuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LiveQuizRooms_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LiveQuizRooms_Users_HostUserId",
                        column: x => x.HostUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LiveQuizParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LiveQuizRoomId = table.Column<int>(type: "int", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsConnected = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveQuizParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveQuizParticipants_LiveQuizRooms_LiveQuizRoomId",
                        column: x => x.LiveQuizRoomId,
                        principalTable: "LiveQuizRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LiveQuizParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiveQuizParticipants_LiveQuizRoomId",
                table: "LiveQuizParticipants",
                column: "LiveQuizRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveQuizParticipants_UserId",
                table: "LiveQuizParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveQuizRooms_CurrentQuestionId",
                table: "LiveQuizRooms",
                column: "CurrentQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveQuizRooms_HostUserId",
                table: "LiveQuizRooms",
                column: "HostUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveQuizRooms_QuizId",
                table: "LiveQuizRooms",
                column: "QuizId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiveQuizParticipants");

            migrationBuilder.DropTable(
                name: "LiveQuizRooms");
        }
    }
}
