using MediatR;

namespace JobTracker.Application.JobApplications.Commands.UploadCv;

public record UploadCvCommand(Guid ApplicationId, string UserId, string FileName, Stream FileStream) : IRequest;
