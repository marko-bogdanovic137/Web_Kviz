using KvizHub.Core.Services.Interfaces;
using KvizHub.Infrastructure.Data.Seed;
using KvizHub.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Infrastructure
{
	public class ApplicationDbContext : DbContext, IApplicationDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		// DbSet-ovi
		public DbSet<User> Users { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Quiz> Quizzes { get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<AnswerOption> AnswerOptions { get; set; }
		public DbSet<QuizResult> QuizResults { get; set; }
		public DbSet<UserAnswer> UserAnswers { get; set; }
		public DbSet<SelectedAnswer> SelectedAnswers { get; set; }
        public DbSet<LiveQuizRoom> LiveQuizRooms { get; set; }
        public DbSet<LiveQuizParticipant> LiveQuizParticipants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			DataSeeder.SeedData(modelBuilder);

			// Indeksi
			modelBuilder.Entity<User>()
				.HasIndex(u => u.Username)
				.IsUnique();
			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();

			modelBuilder.Entity<Category>()
				.HasIndex(c => c.Name)
				.IsUnique();

			modelBuilder.Entity<Quiz>()
				.HasIndex(q => q.Title);

			// Globalno: NO CASCADE DELETE za sve FK-ove
			foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			{
				fk.DeleteBehavior = DeleteBehavior.Restrict;
			}

			// Lokalni izuzeci gde želimo kaskadno brisanje
			modelBuilder.Entity<AnswerOption>()
				.HasOne(ao => ao.Question)
				.WithMany(q => q.AnswerOptions)
				.HasForeignKey(ao => ao.QuestionId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<SelectedAnswer>()
				.HasOne(sa => sa.UserAnswer)
				.WithMany(ua => ua.SelectedAnswers)
				.HasForeignKey(sa => sa.UserAnswerId)
				.OnDelete(DeleteBehavior.Cascade);

			// Konfiguracija veza
			modelBuilder.Entity<Quiz>()
				.HasOne(q => q.Category)
				.WithMany(c => c.Quizzes)
				.HasForeignKey(q => q.CategoryId);

			modelBuilder.Entity<Quiz>()
				.HasOne(q => q.CreatedByUser)
				.WithMany(u => u.CreatedQuizzes)
				.HasForeignKey(q => q.CreatedByUserId);

			modelBuilder.Entity<Question>()
				.HasOne(q => q.Quiz)
				.WithMany(qz => qz.Questions)
				.HasForeignKey(q => q.QuizId);

			modelBuilder.Entity<QuizResult>()
				.HasOne(qr => qr.User)
				.WithMany(u => u.QuizResults)
				.HasForeignKey(qr => qr.UserId);

			modelBuilder.Entity<QuizResult>()
				.HasOne(qr => qr.Quiz)
				.WithMany(q => q.QuizResults)
				.HasForeignKey(qr => qr.QuizId);

			modelBuilder.Entity<UserAnswer>()
				.HasOne(ua => ua.QuizResult)
				.WithMany(qr => qr.UserAnswers)
				.HasForeignKey(ua => ua.QuizResultId);

			modelBuilder.Entity<SelectedAnswer>()
				.HasOne(sa => sa.AnswerOption)
				.WithMany()
				.HasForeignKey(sa => sa.AnswerOptionId);
		}
	}
}
