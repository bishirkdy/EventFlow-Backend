using FluentValidation;

namespace EventFlow.Operations.Application.Features.AttendanceStaff.Commands.RevokeAttendanceStaff;

public sealed class RevokeAttendanceStaffCommandValidator
    : AbstractValidator<RevokeAttendanceStaffCommand>
{
    public RevokeAttendanceStaffCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.AssignmentId)
            .NotEmpty()
            .WithMessage("Assignment ID is required.");
    }
}
