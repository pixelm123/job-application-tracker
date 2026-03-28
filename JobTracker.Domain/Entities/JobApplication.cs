using JobTracker.Domain.Enums;
using JobTracker.Domain.Exceptions;

namespace JobTracker.Domain.Entities;

public class JobApplication
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string CompanyName { get; private set; } = string.Empty;
    public string JobTitle { get; private set; } = string.Empty;
    public string? JobUrl { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public DateTime AppliedDate { get; private set; }
    public string? Notes { get; private set; }
    public string? CvFileName { get; private set; }
    public string? CvFilePath { get; private set; }
    public DateTime? ReminderDate { get; private set; }
    public bool ReminderSent { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public ICollection<Interview> Interviews { get; private set; } = [];

    private JobApplication() { }

    public static JobApplication Create(
        string userId,
        string companyName,
        string jobTitle,
        DateTime appliedDate,
        string? jobUrl = null,
        string? notes = null,
        DateTime? reminderDate = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new DomainException("Company name is required.");

        if (string.IsNullOrWhiteSpace(jobTitle))
            throw new DomainException("Job title is required.");

        var now = DateTime.UtcNow;
        return new JobApplication
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyName = companyName,
            JobTitle = jobTitle,
            JobUrl = jobUrl,
            Status = ApplicationStatus.Applied,
            AppliedDate = appliedDate,
            Notes = notes,
            ReminderDate = reminderDate,
            ReminderSent = false,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Update(
        string companyName,
        string jobTitle,
        string? jobUrl,
        DateTime appliedDate,
        string? notes,
        DateTime? reminderDate)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new DomainException("Company name is required.");

        if (string.IsNullOrWhiteSpace(jobTitle))
            throw new DomainException("Job title is required.");

        CompanyName = companyName;
        JobTitle = jobTitle;
        JobUrl = jobUrl;
        AppliedDate = appliedDate;
        Notes = notes;
        ReminderDate = reminderDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ApplicationStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AttachCv(string fileName, string filePath)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("CV file name is required.");

        CvFileName = fileName;
        CvFilePath = filePath;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkReminderSent()
    {
        ReminderSent = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
