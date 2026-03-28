using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<JobApplication> JobApplications { get; }
    DbSet<Interview> Interviews { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
