using JobTracker.Domain.Enums;
using MediatR;

namespace JobTracker.Application.JobApplications.Commands.UpdateStatus;

public record UpdateStatusCommand(Guid Id, string UserId, ApplicationStatus Status) : IRequest;
