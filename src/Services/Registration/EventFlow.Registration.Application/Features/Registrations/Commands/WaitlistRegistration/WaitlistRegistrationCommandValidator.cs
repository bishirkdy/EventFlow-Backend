using FluentValidation;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.WaitlistRegistration;

public sealed class WaitlistRegistrationCommandValidator
    : AbstractValidator<WaitlistRegistrationCommand>
{
    public WaitlistRegistrationCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.RegistrationId)
            .NotEmpty()
            .WithMessage("Registration ID is required.");
    }
}
