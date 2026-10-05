using FluentValidation;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.ApproveRegistration;

public sealed class ApproveRegistrationCommandValidator
    : AbstractValidator<ApproveRegistrationCommand>
{
    public ApproveRegistrationCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.RegistrationId)
            .NotEmpty()
            .WithMessage("Registration ID is required.");
    }
}
