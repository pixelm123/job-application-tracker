using MediatR;

namespace JobTracker.Application.JobApplications.Commands.DeleteJobApplication;

public record DeleteJobApplicationCommand(Guid Id, string UserId) : IRequest;
