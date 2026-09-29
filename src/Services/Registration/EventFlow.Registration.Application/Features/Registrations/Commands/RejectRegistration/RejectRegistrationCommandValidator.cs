using FluentValidation;
namespace EventFlow.Registration.Application.Features.Registrations.Commands.RejectRegistration;

public sealed class RejectRegistrationCommandValidator : AbstractValidator<RejectRegistrationCommand>
{
    public RejectRegistrationCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.RegistrationId).NotEmpty();
        RuleFor(x => x.Request.Reason).NotEmpty().MaximumLength(1000);
    }
}
