using FluentValidation;

namespace EventFlow.Operations.Application.Features.AttendanceStaff.Queries.GetAttendanceStaff;

public sealed class GetAttendanceStaffQueryValidator
    : AbstractValidator<GetAttendanceStaffQuery>
{
    public GetAttendanceStaffQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
