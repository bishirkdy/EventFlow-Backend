using FluentValidation;

namespace EventFlow.Operations.Application.Features.Attendance.Queries.GetAttendanceDashboard;

public sealed class GetAttendanceDashboardQueryValidator
    : AbstractValidator<GetAttendanceDashboardQuery>
{
    public GetAttendanceDashboardQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
