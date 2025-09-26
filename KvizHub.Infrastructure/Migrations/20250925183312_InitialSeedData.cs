using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KvizHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Kvizovi o programiranju i IT-u", "Programiranje" },
                    { 2, "Istorijski događaji i ličnosti", "Istorija" },
                    { 3, "Naučna dostignuća i otkrića", "Nauka" },
                    { 4, "Sportske teme i takmičenja", "Sport" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "PasswordHash", "ProfileImage", "Username" },
                values: new object[,]
                {
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@kvizhub.com", "$2a$11$LQv3c1yqBWVHrnG0e8M/4e6u6t6Q1V8cY8QaJ8c6vY6dL9rV8cY8Qa", null, "admin" },
                    { 20, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "marko@example.com", "$2a$11$KJv3c1yqBWVHrnG0e8M/4e6u6t6Q1V8cY8QaJ8c6vY6dL9rV8cY8Qb", null, "marko" }
                });

            migrationBuilder.InsertData(
                table: "Quizzes",
                columns: new[] { "Id", "CategoryId", "CreatedByUserId", "Description", "Difficulty", "IsActive", "TimeLimit", "Title" },
                values: new object[,]
                {
                    { 1, 1, 1, "Osnovni koncepti C# programskog jezika", "Lako", true, 10, "C# Osnove" },
                    { 2, 1, 1, "Osnove web dizajna", "Srednje", true, 15, "HTML i CSS" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "Order", "Points", "QuizId", "Text", "Type" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, "Šta je C#?", "MultipleChoice" },
                    { 2, 2, 1, 1, "Koji tip podatka se koristi za cele brojeve?", "MultipleChoice" }
                });

            migrationBuilder.InsertData(
                table: "AnswerOptions",
                columns: new[] { "Id", "IsCorrect", "Order", "QuestionId", "Text" },
                values: new object[,]
                {
                    { 1, true, 1, 1, "Programski jezik" },
                    { 2, false, 2, 1, "Operativni sistem" },
                    { 3, false, 3, 1, "Baza podataka" },
                    { 4, false, 4, 1, "Web framework" },
                    { 5, true, 1, 2, "int" },
                    { 6, false, 2, 2, "string" },
                    { 7, false, 3, 2, "bool" },
                    { 8, false, 4, 2, "double" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
