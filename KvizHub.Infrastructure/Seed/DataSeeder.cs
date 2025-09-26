using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KvizHub.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KvizHub.Infrastructure.Data.Seed
{
	public static class DataSeeder
	{
		public static void SeedData(ModelBuilder modelBuilder)
		{
			SeedCategories(modelBuilder);
			SeedUsers(modelBuilder);
			SeedQuizzes(modelBuilder);
		}

		private static void SeedCategories(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Category>().HasData(
				new Category { Id = 1, Name = "Programiranje", Description = "Kvizovi o programiranju i IT-u" },
				new Category { Id = 2, Name = "Istorija", Description = "Istorijski događaji i ličnosti" },
				new Category { Id = 3, Name = "Nauka", Description = "Naučna dostignuća i otkrića" },
				new Category { Id = 4, Name = "Sport", Description = "Sportske teme i takmičenja" }
			);
		}

		private static void SeedUsers(ModelBuilder modelBuilder)
		{
			// Koristi FIKSNE vrednosti umesto dynamic
			modelBuilder.Entity<User>().HasData(
				new User
				{
					Id = 10,
					Username = "admin",
					Email = "admin@kvizhub.com",
					// Hardkodovan hash za "admin123"
					PasswordHash = "$2a$11$LQv3c1yqBWVHrnG0e8M/4e6u6t6Q1V8cY8QaJ8c6vY6dL9rV8cY8Qa",
					CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Fiksan datum
				},
				new User
				{
					Id = 20,
					Username = "marko",
					Email = "marko@example.com",
					// Hardkodovan hash za "marko123"
					PasswordHash = "$2a$11$KJv3c1yqBWVHrnG0e8M/4e6u6t6Q1V8cY8QaJ8c6vY6dL9rV8cY8Qb",
					CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Fiksan datum
				}
			);
		}

		private static void SeedQuizzes(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Quiz>().HasData(
				new Quiz
				{
					Id = 1,
					Title = "C# Osnove",
					Description = "Osnovni koncepti C# programskog jezika",
					TimeLimit = 10,
					Difficulty = "Lako",
					CategoryId = 1,
					CreatedByUserId = 1,
					IsActive = true
				},
				new Quiz
				{
					Id = 2,
					Title = "HTML i CSS",
					Description = "Osnove web dizajna",
					TimeLimit = 15,
					Difficulty = "Srednje",
					CategoryId = 1,
					CreatedByUserId = 1,
					IsActive = true
				}
			);

			modelBuilder.Entity<Question>().HasData(
				new Question { Id = 1, Text = "Šta je C#?", Type = "MultipleChoice", Points = 1, Order = 1, QuizId = 1 },
				new Question { Id = 2, Text = "Koji tip podatka se koristi za cele brojeve?", Type = "MultipleChoice", Points = 1, Order = 2, QuizId = 1 }
			);

			modelBuilder.Entity<AnswerOption>().HasData(
				new AnswerOption { Id = 1, Text = "Programski jezik", IsCorrect = true, Order = 1, QuestionId = 1 },
				new AnswerOption { Id = 2, Text = "Operativni sistem", IsCorrect = false, Order = 2, QuestionId = 1 },
				new AnswerOption { Id = 3, Text = "Baza podataka", IsCorrect = false, Order = 3, QuestionId = 1 },
				new AnswerOption { Id = 4, Text = "Web framework", IsCorrect = false, Order = 4, QuestionId = 1 },

				new AnswerOption { Id = 5, Text = "int", IsCorrect = true, Order = 1, QuestionId = 2 },
				new AnswerOption { Id = 6, Text = "string", IsCorrect = false, Order = 2, QuestionId = 2 },
				new AnswerOption { Id = 7, Text = "bool", IsCorrect = false, Order = 3, QuestionId = 2 },
				new AnswerOption { Id = 8, Text = "double", IsCorrect = false, Order = 4, QuestionId = 2 }
			);
		}
	}
}