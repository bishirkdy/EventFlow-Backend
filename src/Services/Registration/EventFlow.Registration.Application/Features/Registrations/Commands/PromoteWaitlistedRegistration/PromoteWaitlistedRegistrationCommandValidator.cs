using FluentValidation;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.PromoteWaitlistedRegistration;

public sealed class PromoteWaitlistedRegistrationCommandValidator
    : AbstractValidator<PromoteWaitlistedRegistrationCommand>
{
    public PromoteWaitlistedRegistrationCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.RegistrationId)
            .NotEmpty()
            .WithMessage("Registration ID is required.");
    }
}
