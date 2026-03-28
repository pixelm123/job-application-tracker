using JobTracker.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace JobTracker.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendReminderEmailAsync(
        string toEmail,
        string firstName,
        string companyName,
        string jobTitle,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Job Tracker", _configuration["Smtp:FromEmail"] ?? "noreply@jobtracker.local"));
        message.To.Add(new MailboxAddress(firstName, toEmail));
        message.Subject = $"Reminder: Follow up on your {jobTitle} application at {companyName}";

        message.Body = new TextPart("html")
        {
            Text = $"""
                <h2>Job Application Reminder</h2>
                <p>Hi {firstName},</p>
                <p>This is a reminder to follow up on your application for <strong>{jobTitle}</strong> at <strong>{companyName}</strong>.</p>
                <p>Good luck!</p>
                <p><em>Job Tracker</em></p>
                """
        };

        var host = _configuration["Smtp:Host"];
        var port = int.Parse(_configuration["Smtp:Port"] ?? "587");
        var username = _configuration["Smtp:Username"] ?? "";
        var password = _configuration["Smtp:Password"] ?? "";

        // Mailpit (local dev) uses plain socket, no TLS, no auth
        var isLocalDev = host == "localhost" || host == "127.0.0.1";
        var secureOption = isLocalDev
            ? MailKit.Security.SecureSocketOptions.None
            : MailKit.Security.SecureSocketOptions.StartTls;

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, secureOption, cancellationToken);

        if (!isLocalDev && !string.IsNullOrEmpty(username))
            await client.AuthenticateAsync(username, password, cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
