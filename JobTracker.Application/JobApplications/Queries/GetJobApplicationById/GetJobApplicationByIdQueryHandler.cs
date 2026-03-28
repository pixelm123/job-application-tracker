using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.JobApplications.Queries.GetJobApplications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Application.JobApplications.Queries.GetJobApplicationById;

public class GetJobApplicationByIdQueryHandler
    : IRequestHandler<GetJobApplicationByIdQuery, JobApplicationDto?>
{
    private readonly IApplicationDbContext _context;

    public GetJobApplicationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobApplicationDto?> Handle(
        GetJobApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.JobApplications
            .Include(x => x.Interviews)
            .Where(x => x.Id == request.Id && x.UserId == request.UserId)
            .Select(x => new JobApplicationDto(
                x.Id,
                x.CompanyName,
                x.JobTitle,
                x.JobUrl,
                x.Status.ToString(),
                x.AppliedDate,
                x.Notes,
                x.CvFileName,
                x.ReminderDate,
                x.ReminderSent,
                x.CreatedAt,
                x.UpdatedAt,
                x.Interviews.Select(i => new InterviewDto(
                    i.Id,
                    i.ScheduledAt,
                    i.Type.ToString(),
                    i.Notes,
                    i.CreatedAt)).ToList(),
                x.CvFilePath))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
