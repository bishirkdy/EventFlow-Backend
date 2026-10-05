using FluentValidation;

namespace EventFlow.Operations.Application.Features.Attendance.Queries.GetParticipantAttendanceHistory;

public sealed class GetParticipantAttendanceHistoryQueryValidator
    : AbstractValidator<GetParticipantAttendanceHistoryQuery>
{
    public GetParticipantAttendanceHistoryQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.ParticipantUserId)
            .NotEmpty()
            .WithMessage("Participant user ID is required.");
    }
}
