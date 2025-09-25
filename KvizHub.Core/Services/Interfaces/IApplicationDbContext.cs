using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KvizHub.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KvizHub.Core.Services.Interfaces
{
	public interface IApplicationDbContext
	{
		DbSet<User> Users { get; set; }
		DbSet<Category> Categories { get; set; }
		DbSet<Quiz> Quizzes { get; set; }
		DbSet<Question> Questions { get; set; }
		DbSet<AnswerOption> AnswerOptions { get; set; }
		DbSet<QuizResult> QuizResults { get; set; }
		DbSet<UserAnswer> UserAnswers { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
