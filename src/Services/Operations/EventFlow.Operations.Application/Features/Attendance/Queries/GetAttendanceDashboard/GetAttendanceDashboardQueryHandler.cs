using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.Attendance.Queries.GetAttendanceDashboard;

public sealed class GetAttendanceDashboardQueryHandler(
    IOperationsDbContext db,
    IEventAuthorizationClient authorization,
    ICurrentUserService currentUser)
    : IRequestHandler<GetAttendanceDashboardQuery, AttendanceDashboardDto>
{
    public async Task<AttendanceDashboardDto> Handle(
        GetAttendanceDashboardQuery query,
        CancellationToken cancellationToken)
    {
        if (!await authorization.HasPermissionAsync(
                currentUser.UserId,
                query.EventId,
                "event.view",
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission to view attendance.");
        }

        var records = await db.AttendanceRecords
            .Where(x => x.EventId == query.EventId)
            .ToListAsync(cancellationToken);

        var distinct = records.Select(x => x.RegistrationId).Distinct().Count();
        var checkedOut = records.Count(x => x.CheckedOutAtUtc.HasValue);
        var inside = records.Count(x => !x.CheckedOutAtUtc.HasValue);

        return new AttendanceDashboardDto(
            distinct,
            distinct,
            checkedOut,
            inside,
            distinct == 0 ? 0 : 100d);
    }
}
