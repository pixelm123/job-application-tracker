using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.JobApplications.Queries.GetJobApplications;
using JobTracker.Domain.Entities;
using MediatR;

namespace JobTracker.Application.JobApplications.Commands.CreateJobApplication;

public class CreateJobApplicationCommandHandler
    : IRequestHandler<CreateJobApplicationCommand, JobApplicationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public CreateJobApplicationCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<JobApplicationDto> Handle(
        CreateJobApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = JobApplication.Create(
            request.UserId,
            request.CompanyName,
            request.JobTitle,
            request.AppliedDate,
            request.JobUrl,
            request.Notes,
            request.ReminderDate);

        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync($"applications:{request.UserId}:p20", cancellationToken);

        return new JobApplicationDto(
            application.Id,
            application.CompanyName,
            application.JobTitle,
            application.JobUrl,
            application.Status.ToString(),
            application.AppliedDate,
            application.Notes,
            application.CvFileName,
            application.ReminderDate,
            application.ReminderSent,
            application.CreatedAt,
            application.UpdatedAt,
            []);
    }
}
