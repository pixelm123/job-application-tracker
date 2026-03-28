using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.JobApplications.Queries.GetJobApplications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Application.JobApplications.Commands.UpdateJobApplication;

public class UpdateJobApplicationCommandHandler
    : IRequestHandler<UpdateJobApplicationCommand, JobApplicationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public UpdateJobApplicationCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<JobApplicationDto> Handle(
        UpdateJobApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.JobApplications
            .Include(x => x.Interviews)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId, cancellationToken);

        if (application is null)
            throw new KeyNotFoundException($"Application {request.Id} not found.");

        application.Update(
            request.CompanyName,
            request.JobTitle,
            request.JobUrl,
            request.AppliedDate,
            request.Notes,
            request.ReminderDate);

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
            application.Interviews.Select(i => new InterviewDto(
                i.Id, i.ScheduledAt, i.Type.ToString(), i.Notes, i.CreatedAt)).ToList(),
            application.CvFilePath);
    }
}
