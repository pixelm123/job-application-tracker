using System.Text;
using Hangfire;
using JobTracker.API.Middleware;
using JobTracker.Application;
using JobTracker.Infrastructure;
using JobTracker.Infrastructure.Jobs;
using JobTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured. Set it in appsettings or environment variables.");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Job Tracker API";
    options.Theme = ScalarTheme.DeepSpace;
    options.AddServer("https://job-application-tracker-8sqx.onrender.com");
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<ReminderJob>(
    "daily-reminders",
    job => job.ExecuteAsync(),
    Cron.Daily);

app.MapControllers();

app.Run();
