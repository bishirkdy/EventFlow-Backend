using FluentValidation;

namespace EventFlow.Operations.Application.Features.Attendance.Commands.CheckOut;

public sealed class CheckOutCommandValidator : AbstractValidator<CheckOutCommand>
{
    public CheckOutCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.AttendanceId)
            .NotEmpty()
            .WithMessage("Attendance ID is required.");
    }
}
