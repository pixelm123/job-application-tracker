using JobTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Application.JobApplications.Commands.UploadCv;

public class UploadCvCommandHandler : IRequestHandler<UploadCvCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;
    private const string UploadDir = "uploads/cv";

    public UploadCvCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task Handle(UploadCvCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.JobApplications
            .FirstOrDefaultAsync(x => x.Id == request.ApplicationId && x.UserId == request.UserId, cancellationToken);

        if (application is null)
            throw new KeyNotFoundException($"Application {request.ApplicationId} not found.");

        Directory.CreateDirectory(UploadDir);
        var safeFileName = $"{request.ApplicationId}_{Path.GetFileName(request.FileName)}";
        var filePath = Path.Combine(UploadDir, safeFileName);

        await using var fileStream = File.Create(filePath);
        await request.FileStream.CopyToAsync(fileStream, cancellationToken);

        application.AttachCv(request.FileName, filePath);
        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveAsync($"applications:{request.UserId}:p20", cancellationToken);
    }
}
