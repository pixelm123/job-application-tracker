using JobTracker.Application.Common.Interfaces;
using JobTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobTracker.Infrastructure.Jobs;

public class ReminderJob
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ReminderJob> _logger;

    public ReminderJob(
        IApplicationDbContext context,
        IEmailService emailService,
        UserManager<ApplicationUser> userManager,
        ILogger<ReminderJob> logger)
    {
        _context = context;
        _emailService = emailService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var today = DateTime.UtcNow.Date;

        var dueApplications = await _context.JobApplications
            .Where(x => x.ReminderDate.HasValue
                && x.ReminderDate.Value.Date <= today
                && !x.ReminderSent)
            .ToListAsync();

        foreach (var application in dueApplications)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(application.UserId);
                if (user is null) continue;

                await _emailService.SendReminderEmailAsync(
                    user.Email!,
                    user.FirstName,
                    application.CompanyName,
                    application.JobTitle);

                application.MarkReminderSent();
                _logger.LogInformation("Reminder sent for application {Id} to {Email}", application.Id, user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reminder for application {Id}", application.Id);
            }
        }

        await _context.SaveChangesAsync();
    }
}
