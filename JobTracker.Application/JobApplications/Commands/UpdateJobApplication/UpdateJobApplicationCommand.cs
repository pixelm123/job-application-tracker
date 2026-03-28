using JobTracker.Application.JobApplications.Queries.GetJobApplications;
using MediatR;

namespace JobTracker.Application.JobApplications.Commands.UpdateJobApplication;

public record UpdateJobApplicationCommand(
    Guid Id,
    string UserId,
    string CompanyName,
    string JobTitle,
    string? JobUrl,
    DateTime AppliedDate,
    string? Notes,
    DateTime? ReminderDate) : IRequest<JobApplicationDto>;
