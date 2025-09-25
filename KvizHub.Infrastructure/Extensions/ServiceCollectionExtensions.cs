using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KvizHub.Infrastructure.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			// Database Context
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

			// Dodati ćemo ove kasnije kada budemo imali repozitorijume
			// services.AddScoped<IUserRepository, UserRepository>();
			// services.AddScoped<IQuizRepository, QuizRepository>();

			return services;
		}
	}
}