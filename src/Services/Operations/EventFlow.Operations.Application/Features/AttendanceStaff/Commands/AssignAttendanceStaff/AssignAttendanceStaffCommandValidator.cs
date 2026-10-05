using EventFlow.Operations.Domain.Enums;
using FluentValidation;

namespace EventFlow.Operations.Application.Features.AttendanceStaff.Commands.AssignAttendanceStaff;

public sealed class AssignAttendanceStaffCommandValidator
    : AbstractValidator<AssignAttendanceStaffCommand>
{
    public AssignAttendanceStaffCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email address is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(320)
            .WithMessage("Email address cannot exceed 320 characters.");

        RuleFor(x => x.ScopeType)
            .IsInEnum()
            .WithMessage("Invalid attendance scope type.");

        RuleFor(x => x.ScopeId)
            .NotEmpty()
            .When(x => x.ScopeType != AttendanceScopeType.Event)
            .WithMessage("ScopeId is required for section and session attendance staff.");
    }
}
