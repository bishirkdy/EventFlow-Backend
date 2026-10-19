using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Enums;
using EventFlow.Security.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.Attendance;

internal enum AttendanceAccess
{
    None = 0,
    SessionExact = 1,
    Broad = 2,
}

internal static class AttendanceAuthorization
{
    public static async Task<AttendanceAccess> ResolveAsync(
        IOperationsDbContext db,
        IEventAuthorizationClient authorization,
        Guid eventId,
        Guid userId,
        Guid? sessionId,
        CancellationToken cancellationToken)
    {
        if (await authorization.HasPermissionAsync(
                userId, eventId, PermissionConstants.Event.TeamManage, cancellationToken))
        {
            return AttendanceAccess.Broad;
        }

        var assignments = await db.AttendanceStaffAssignments
            .Where(x => x.EventId == eventId && x.UserId == userId && x.IsActive)
            .Select(x => new { x.ScopeType, x.ScopeId })
            .ToListAsync(cancellationToken);

        if (assignments.Any(x => x.ScopeType == AttendanceScopeType.Event))
        {
            return AttendanceAccess.Broad;
        }

        if (sessionId.HasValue &&
            assignments.Any(x => x.ScopeType == AttendanceScopeType.Session && x.ScopeId == sessionId))
        {
            return AttendanceAccess.SessionExact;
        }

        return AttendanceAccess.None;
    }
}
