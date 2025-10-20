using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KvizHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ZaLaptop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsAdmin", "PasswordHash", "ProfileImage", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@kvizhub.com", false, "$2a$11$LQv3c1yqBWVHrnG0e8M/4e6u6t6Q1V8cY8QaJ8c6vY6dL9rV8cY8Qa", null, "admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "marko@example.com", false, "$2a$11$KJv3c1yqBWVHrnG0e8M/4e6u6t6Q1V8cY8QaJ8c6vY6dL9rV8cY8Qb", null, "marko" }
                });
        }
    }
}
