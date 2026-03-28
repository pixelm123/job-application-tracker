using JobTracker.Application.JobApplications.Queries.GetJobApplications;
using MediatR;

namespace JobTracker.Application.JobApplications.Queries.GetJobApplicationById;

public record GetJobApplicationByIdQuery(Guid Id, string UserId) : IRequest<JobApplicationDto?>;
