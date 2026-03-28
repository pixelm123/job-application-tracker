using JobTracker.Application.JobApplications.Queries.GetJobApplications;
using MediatR;

namespace JobTracker.Application.JobApplications.Commands.CreateJobApplication;

public record CreateJobApplicationCommand(
    string UserId,
    string CompanyName,
    string JobTitle,
    string? JobUrl,
    DateTime AppliedDate,
    string? Notes,
    DateTime? ReminderDate) : IRequest<JobApplicationDto>;
