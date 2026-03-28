using JobTracker.Domain.Enums;
using JobTracker.Domain.Exceptions;

namespace JobTracker.Domain.Entities;

public class Interview
{
    public Guid Id { get; private set; }
    public Guid JobApplicationId { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public InterviewType Type { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public JobApplication JobApplication { get; private set; } = null!;

    private Interview() { }

    public static Interview Create(
        Guid jobApplicationId,
        DateTime scheduledAt,
        InterviewType type,
        string? notes = null)
    {
        if (scheduledAt < DateTime.UtcNow.AddMinutes(-5))
            throw new DomainException("Interview cannot be scheduled in the past.");

        return new Interview
        {
            Id = Guid.NewGuid(),
            JobApplicationId = jobApplicationId,
            ScheduledAt = scheduledAt,
            Type = type,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(DateTime scheduledAt, InterviewType type, string? notes)
    {
        ScheduledAt = scheduledAt;
        Type = type;
        Notes = notes;
    }
}
