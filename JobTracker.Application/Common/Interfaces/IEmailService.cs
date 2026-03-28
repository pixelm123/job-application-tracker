namespace JobTracker.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendReminderEmailAsync(string toEmail, string firstName, string companyName, string jobTitle, CancellationToken cancellationToken = default);
}
