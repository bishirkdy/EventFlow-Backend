using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.Attendance;

internal static class AttendanceAuthorization
{
    public static async Task<bool> CanOperateAsync(
        IOperationsDbContext db,
        IEventAuthorizationClient authorization,
        Guid eventId,
        Guid userId,
        Guid? sectionId,
        Guid? sessionId,
        CancellationToken cancellationToken)
    {
        if (await authorization.HasPermissionAsync(
                userId, eventId, "event.team.manage", cancellationToken))
        {
            return true;
        }

        return await db.AttendanceStaffAssignments.AnyAsync(
            x => x.EventId == eventId
                 && x.UserId == userId
                 && x.IsActive
                 && (x.ScopeType == AttendanceScopeType.Event
                     || (x.ScopeType == AttendanceScopeType.Section && x.ScopeId == sectionId)
                     || (x.ScopeType == AttendanceScopeType.Session && x.ScopeId == sessionId)),
            cancellationToken);
    }
}
