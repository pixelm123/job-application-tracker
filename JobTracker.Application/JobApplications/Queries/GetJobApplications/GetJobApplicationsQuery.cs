using JobTracker.Application.Common.Models;
using JobTracker.Domain.Enums;
using MediatR;

namespace JobTracker.Application.JobApplications.Queries.GetJobApplications;

public record GetJobApplicationsQuery(
    string UserId,
    int Page = 1,
    int PageSize = 20,
    ApplicationStatus? Status = null,
    string? Search = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null) : IRequest<PaginatedList<JobApplicationDto>>;

public record JobApplicationDto(
    Guid Id,
    string CompanyName,
    string JobTitle,
    string? JobUrl,
    string Status,
    DateTime AppliedDate,
    string? Notes,
    string? CvFileName,
    DateTime? ReminderDate,
    bool ReminderSent,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<InterviewDto> Interviews,
    string? CvFilePath = null);

public record InterviewDto(
    Guid Id,
    DateTime ScheduledAt,
    string Type,
    string? Notes,
    DateTime CreatedAt);
