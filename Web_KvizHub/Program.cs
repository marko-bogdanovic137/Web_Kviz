using KvizHub.Infrastructure;
using KvizHub.Infrastructure.Extensions;
using KvizHub.Infrastructure.Services.Implementations;
using KvizHub.Infrastructure.Services.Interfaces;
using KvizHub.Infrastructure.Data;
using KvizHub.Infrastructure.Mapping;
using Web_KvizHub.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program)); // Za API
builder.Services.AddAutoMapper(typeof(MappingProfile)); // Za Infrastructure

// Dodaj našu infrastrukturu
builder.Services.AddInfrastructure(builder.Configuration);

// Registruj servise
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuizSolvingService, QuizSolvingService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();

var app = builder.Build();

app.UseMiddleware<JwtMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

