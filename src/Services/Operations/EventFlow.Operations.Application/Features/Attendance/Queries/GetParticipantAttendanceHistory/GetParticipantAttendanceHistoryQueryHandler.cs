using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.Attendance.Queries.GetParticipantAttendanceHistory;

public sealed class GetParticipantAttendanceHistoryQueryHandler(
    IOperationsDbContext db,
    IEventAuthorizationClient authorization,
    IEventScheduleClient scheduleClient,
    ICurrentUserService currentUser)
    : IRequestHandler<GetParticipantAttendanceHistoryQuery, AttendanceHistoryDto>
{
    public async Task<AttendanceHistoryDto> Handle(
        GetParticipantAttendanceHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var actorUserId = currentUser.UserId;

        if (actorUserId != query.ParticipantUserId
            && !await authorization.HasPermissionAsync(
                actorUserId, query.EventId, "event.view", cancellationToken))
        {
            throw new ForbiddenException("You do not have access to this attendance history.");
        }

        var records = await db.AttendanceRecords
            .Where(x => x.EventId == query.EventId
                        && x.ParticipantUserId == query.ParticipantUserId)
            .ToListAsync(cancellationToken);

        var totalSessions = await scheduleClient.GetSessionCountAsync(
            query.EventId,
            cancellationToken);

        var attendedSessions = records.Count(x => x.SessionId.HasValue);
        var percentage = totalSessions == 0
            ? 0
            : Math.Round(attendedSessions * 100d / totalSessions, 2);

        return new AttendanceHistoryDto(
            query.EventId,
            records.Count(x => x.SessionId == null),
            attendedSessions,
            totalSessions,
            percentage);
    }
}
