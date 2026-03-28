using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Common.Models;
using JobTracker.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace JobTracker.Application.JobApplications.Queries.GetJobApplications;

public class GetJobApplicationsQueryHandler
    : IRequestHandler<GetJobApplicationsQuery, PaginatedList<JobApplicationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public GetJobApplicationsQueryHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<PaginatedList<JobApplicationDto>> Handle(
        GetJobApplicationsQuery request, CancellationToken cancellationToken)
    {
        // Only cache unfiltered first-page requests (the common dashboard/kanban load)
        var useCache = request.Page == 1
            && request.Status is null
            && string.IsNullOrEmpty(request.Search)
            && request.FromDate is null
            && request.ToDate is null;

        if (useCache)
        {
            var cacheKey = $"applications:{request.UserId}:p{request.PageSize}";
            var cached = await _cache.GetAsync<PaginatedList<JobApplicationDto>>(cacheKey, cancellationToken);
            if (cached is not null) return cached;

            var result = await FetchFromDb(request, cancellationToken);
            await _cache.SetAsync(cacheKey, result, cancellationToken: cancellationToken);
            return result;
        }

        return await FetchFromDb(request, cancellationToken);
    }

    private async Task<PaginatedList<JobApplicationDto>> FetchFromDb(
        GetJobApplicationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.JobApplications
            .Include(x => x.Interviews)
            .Where(x => x.UserId == request.UserId);

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        if (!string.IsNullOrEmpty(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(x =>
                x.CompanyName.ToLower().Contains(search) ||
                x.JobTitle.ToLower().Contains(search));
        }

        if (request.FromDate.HasValue)
            query = query.Where(x => x.AppliedDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.AppliedDate <= request.ToDate.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);

        return new PaginatedList<JobApplicationDto>(items, total, request.Page, request.PageSize);
    }
}
