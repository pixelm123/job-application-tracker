using JobTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Application.JobApplications.Commands.UpdateStatus;

public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public UpdateStatusCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.JobApplications
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId, cancellationToken);

        if (application is null)
            throw new KeyNotFoundException($"Application {request.Id} not found.");

        application.UpdateStatus(request.Status);
        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveAsync($"applications:{request.UserId}:p20", cancellationToken);
    }
}
