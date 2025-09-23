using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KvizHub.Models.Entities;

namespace KvizHub.Infrastructure.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		// DbSet-ovi za sve entitete
		public DbSet<User> Users { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Quiz> Quizzes { get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<AnswerOption> AnswerOptions { get; set; }
		public DbSet<QuizResult> QuizResults { get; set; }
		public DbSet<UserAnswer> UserAnswers { get; set; }
		public DbSet<SelectedAnswer> SelectedAnswers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Konfiguracija odnosa i constraints
			ConfigureUser(modelBuilder);
			ConfigureQuiz(modelBuilder);
			ConfigureQuestion(modelBuilder);
			ConfigureQuizResult(modelBuilder);
		}

		private void ConfigureUser(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(entity =>
			{
				entity.HasIndex(u => u.Username).IsUnique();
				entity.HasIndex(u => u.Email).IsUnique();

				entity.HasMany(u => u.QuizResults)
					  .WithOne(qr => qr.User)
					  .HasForeignKey(qr => qr.UserId)
					  .OnDelete(DeleteBehavior.Cascade);

				entity.HasMany(u => u.CreatedQuizzes)
					  .WithOne(q => q.CreatedByUser)
					  .HasForeignKey(q => q.CreatedByUserId)
					  .OnDelete(DeleteBehavior.Restrict);
			});
		}

		private void ConfigureQuiz(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Quiz>(entity =>
			{
				entity.HasIndex(q => q.Title);

				entity.HasOne(q => q.Category)
					  .WithMany(c => c.Quizzes)
					  .HasForeignKey(q => q.CategoryId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasMany(q => q.Questions)
					  .WithOne(qu => qu.Quiz)
					  .HasForeignKey(qu => qu.QuizId)
					  .OnDelete(DeleteBehavior.Cascade);

				entity.HasMany(q => q.QuizResults)
					  .WithOne(qr => qr.Quiz)
					  .HasForeignKey(qr => qr.QuizId)
					  .OnDelete(DeleteBehavior.Cascade);
			});
		}

		private void ConfigureQuestion(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Question>(entity =>
			{
				entity.HasMany(q => q.AnswerOptions)
					  .WithOne(ao => ao.Question)
					  .HasForeignKey(ao => ao.QuestionId)
					  .OnDelete(DeleteBehavior.Cascade);
			});
		}

		private void ConfigureQuizResult(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<QuizResult>(entity =>
			{
				entity.HasMany(qr => qr.UserAnswers)
					  .WithOne(ua => ua.QuizResult)
					  .HasForeignKey(ua => ua.QuizResultId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<UserAnswer>(entity =>
			{
				entity.HasMany(ua => ua.SelectedAnswers)
					  .WithOne(sa => sa.UserAnswer)
					  .HasForeignKey(sa => sa.UserAnswerId)
					  .OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
