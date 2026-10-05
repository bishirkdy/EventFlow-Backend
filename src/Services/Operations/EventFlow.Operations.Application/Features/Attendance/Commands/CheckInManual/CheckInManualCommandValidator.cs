using FluentValidation;

namespace EventFlow.Operations.Application.Features.Attendance.Commands.CheckInManual;

public sealed class CheckInManualCommandValidator
    : AbstractValidator<CheckInManualCommand>
{
    public CheckInManualCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.RegistrationId)
            .NotEmpty()
            .WithMessage("Registration ID is required.");
    }
}
