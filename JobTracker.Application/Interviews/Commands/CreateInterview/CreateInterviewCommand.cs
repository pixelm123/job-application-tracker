using JobTracker.Application.JobApplications.Queries.GetJobApplications;
using JobTracker.Domain.Enums;
using MediatR;

namespace JobTracker.Application.Interviews.Commands.CreateInterview;

public record CreateInterviewCommand(
    Guid JobApplicationId,
    string UserId,
    DateTime ScheduledAt,
    InterviewType Type,
    string? Notes) : IRequest<InterviewDto>;
