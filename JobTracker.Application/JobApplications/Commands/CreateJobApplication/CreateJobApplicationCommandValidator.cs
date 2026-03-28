using FluentValidation;

namespace JobTracker.Application.JobApplications.Commands.CreateJobApplication;

public class CreateJobApplicationCommandValidator : AbstractValidator<CreateJobApplicationCommand>
{
    public CreateJobApplicationCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.JobUrl).MaximumLength(2000).When(x => x.JobUrl is not null);
        RuleFor(x => x.AppliedDate).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow.AddDays(1));
        RuleFor(x => x.Notes).MaximumLength(5000).When(x => x.Notes is not null);
    }
}
