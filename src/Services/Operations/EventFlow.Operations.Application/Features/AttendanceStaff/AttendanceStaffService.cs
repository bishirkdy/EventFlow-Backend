using EventFlow.Contracts.Common;
using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Entities;
using EventFlow.Operations.Domain.Enums;

namespace EventFlow.Operations.Application.Features.AttendanceStaff;

public sealed class AttendanceStaffService(IOperationsDbContext db, IEventAuthorizationClient authorization)
{
    public async Task<ApiResponse<AttendanceStaffDto>> AssignAsync(Guid eventId, Guid actorUserId, AssignAttendanceStaffRequest request, CancellationToken ct)
    {
        if (!await authorization.HasPermissionAsync(actorUserId, eventId, "event.team.manage", ct))
            return ApiResponse<AttendanceStaffDto>.Fail(["You do not have permission to manage attendance staff."]);

        if (request.ScopeType != AttendanceScopeType.Event && request.ScopeId is null)
            return ApiResponse<AttendanceStaffDto>.Fail(["ScopeId is required for section and session attendance staff."]);

        var exists = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            db.AttendanceStaffAssignments,
            x => x.EventId == eventId && x.UserId == request.UserId && x.ScopeType == request.ScopeType && x.ScopeId == request.ScopeId && x.IsActive, ct);
        if (exists is not null)
            return ApiResponse<AttendanceStaffDto>.Success(ToDto(exists), "Attendance staff assignment already exists.");

        var entity = new AttendanceStaffAssignment { EventId = eventId, UserId = request.UserId, ScopeType = request.ScopeType, ScopeId = request.ScopeId };
        db.AttendanceStaffAssignments.Add(entity);
        await db.SaveChangesAsync(ct);
        return ApiResponse<AttendanceStaffDto>.Success(ToDto(entity), "Attendance staff assigned successfully.");
    }

    public async Task<ApiResponse<IReadOnlyList<AttendanceStaffDto>>> ListAsync(Guid eventId, Guid actorUserId, CancellationToken ct)
    {
        if (!await authorization.HasPermissionAsync(actorUserId, eventId, "event.view", ct))
            return ApiResponse<IReadOnlyList<AttendanceStaffDto>>.Fail(["You do not have permission to view attendance staff."]);
        var items = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            db.AttendanceStaffAssignments.Where(x => x.EventId == eventId && x.IsActive).OrderBy(x => x.ScopeType), ct);
        return ApiResponse<IReadOnlyList<AttendanceStaffDto>>.Success(items.Select(ToDto).ToList());
    }

    public async Task<ApiResponse<object?>> RevokeAsync(Guid eventId, Guid assignmentId, Guid actorUserId, CancellationToken ct)
    {
        if (!await authorization.HasPermissionAsync(actorUserId, eventId, "event.team.manage", ct))
            return ApiResponse<object?>.Fail(["You do not have permission to manage attendance staff."]);
        var entity = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(db.AttendanceStaffAssignments, x => x.Id == assignmentId && x.EventId == eventId && x.IsActive, ct);
        if (entity is null) return ApiResponse<object?>.Fail(["Attendance staff assignment not found."]);
        entity.IsActive = false; entity.RevokedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ApiResponse<object?>.Success(null, "Attendance staff assignment revoked.");
    }

    private static AttendanceStaffDto ToDto(AttendanceStaffAssignment x) => new(x.Id,x.EventId,x.UserId,x.ScopeType,x.ScopeId,x.IsActive);
}
