using FluentValidation;
namespace EventFlow.Registration.Application.Features.Registrations.Commands.CancelRegistration;

public sealed class CancelRegistrationCommandValidator : AbstractValidator<CancelRegistrationCommand>
{
    public CancelRegistrationCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.RegistrationId).NotEmpty();
        RuleFor(x => x.Request.Reason).MaximumLength(1000);
    }
}
