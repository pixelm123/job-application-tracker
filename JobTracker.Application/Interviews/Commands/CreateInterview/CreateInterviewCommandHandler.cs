using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.JobApplications.Queries.GetJobApplications;
using JobTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Application.Interviews.Commands.CreateInterview;

public class CreateInterviewCommandHandler : IRequestHandler<CreateInterviewCommand, InterviewDto>
{
    private readonly IApplicationDbContext _context;

    public CreateInterviewCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InterviewDto> Handle(
        CreateInterviewCommand request, CancellationToken cancellationToken)
    {
        var applicationExists = await _context.JobApplications
            .AnyAsync(x => x.Id == request.JobApplicationId && x.UserId == request.UserId, cancellationToken);

        if (!applicationExists)
            throw new KeyNotFoundException($"Application {request.JobApplicationId} not found.");

        var interview = Interview.Create(
            request.JobApplicationId,
            request.ScheduledAt,
            request.Type,
            request.Notes);

        _context.Interviews.Add(interview);
        await _context.SaveChangesAsync(cancellationToken);

        return new InterviewDto(
            interview.Id,
            interview.ScheduledAt,
            interview.Type.ToString(),
            interview.Notes,
            interview.CreatedAt);
    }
}
