using FluentValidation;
using JobTracker.Domain.Enums;

namespace JobTracker.Application.JobApplications.Commands.UpdateStatus;

public class UpdateStatusCommandValidator : AbstractValidator<UpdateStatusCommand>
{
    public UpdateStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
