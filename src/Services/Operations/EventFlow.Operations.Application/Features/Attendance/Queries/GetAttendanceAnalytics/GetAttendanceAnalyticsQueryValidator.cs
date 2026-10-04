using FluentValidation;

namespace EventFlow.Operations.Application.Features.Attendance.Queries.GetAttendanceAnalytics;

internal sealed class GetAttendanceAnalyticsQueryValidator
    : AbstractValidator<GetAttendanceAnalyticsQuery>
{
    public GetAttendanceAnalyticsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Days)
            .InclusiveBetween(1, 90)
            .WithMessage("Days must be between 1 and 90.");
    }
}
