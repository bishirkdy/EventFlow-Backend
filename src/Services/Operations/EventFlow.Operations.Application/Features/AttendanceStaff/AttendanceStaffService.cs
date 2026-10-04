using EventFlow.Contracts.Common;
using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Entities;
using EventFlow.Operations.Domain.Enums;

namespace EventFlow.Operations.Application.Features.AttendanceStaff;

public sealed class AttendanceStaffService(IOperationsDbContext db, IEventAuthorizationClient authorization, IIdentityClient identity)
{
    private const string AttendanceStaffRoleName = "AttendanceStaff";

    public async Task<ApiResponse<AttendanceStaffDto>> AssignAsync(Guid eventId, Guid actorUserId, AssignAttendanceStaffRequest request, CancellationToken ct)
    {
        if (!await authorization.HasPermissionAsync(actorUserId, eventId, "event.team.manage", ct))
            return ApiResponse<AttendanceStaffDto>.Fail(["You do not have permission to manage attendance staff."]);

        var user = await identity.FindByEmailAsync(request.Email, ct);
        if (user is null)
            return ApiResponse<AttendanceStaffDto>.Fail(["No user was found with that email address."]);

        if (request.ScopeType != AttendanceScopeType.Event && request.ScopeId is null)
            return ApiResponse<AttendanceStaffDto>.Fail(["ScopeId is required for section and session attendance staff."]);

        var exists = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            db.AttendanceStaffAssignments,
            x => x.EventId == eventId && x.UserId == user.Id && x.ScopeType == request.ScopeType && x.ScopeId == request.ScopeId && x.IsActive, ct);
        if (exists is not null)
        {
            // Repair older assignments that predate event-role provisioning.
            await identity.AssignEventRoleAsync(user.Id, eventId, AttendanceStaffRoleName, ct);
            return ApiResponse<AttendanceStaffDto>.Success(ToDto(exists, user.Email), "Attendance staff assignment already exists.");
        }

        var entity = new AttendanceStaffAssignment { EventId = eventId, UserId = user.Id, ScopeType = request.ScopeType, ScopeId = request.ScopeId };
        db.AttendanceStaffAssignments.Add(entity);
        await db.SaveChangesAsync(ct);

        // The assignee needs the AttendanceStaff event role so they can open
        // the staff portal (frontend guard + event.view checks).
        var roleAssigned = await identity.AssignEventRoleAsync(user.Id, eventId, AttendanceStaffRoleName, ct);
        if (!roleAssigned)
        {
            db.AttendanceStaffAssignments.Remove(entity);
            await db.SaveChangesAsync(ct);
            return ApiResponse<AttendanceStaffDto>.Fail(["Attendance staff assignment failed because the event role could not be assigned."]);
        }

        return ApiResponse<AttendanceStaffDto>.Success(ToDto(entity, user.Email), "Attendance staff assigned successfully.");
    }

    public async Task<ApiResponse<IReadOnlyList<AttendanceStaffDto>>> ListAsync(Guid eventId, Guid actorUserId, CancellationToken ct)
    {
        if (!await authorization.HasPermissionAsync(actorUserId, eventId, "event.view", ct))
            return ApiResponse<IReadOnlyList<AttendanceStaffDto>>.Fail(["You do not have permission to view attendance staff."]);
        var items = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            db.AttendanceStaffAssignments.Where(x => x.EventId == eventId && x.IsActive).OrderBy(x => x.ScopeType), ct);
        var result = new List<AttendanceStaffDto>(items.Count);
        foreach (var item in items)
        {
            var user = await identity.FindByIdAsync(item.UserId, ct);
            result.Add(ToDto(item, user?.Email ?? "Unknown user"));
        }
        return ApiResponse<IReadOnlyList<AttendanceStaffDto>>.Success(result);
    }

    public async Task<ApiResponse<object?>> RevokeAsync(Guid eventId, Guid assignmentId, Guid actorUserId, CancellationToken ct)
    {
        if (!await authorization.HasPermissionAsync(actorUserId, eventId, "event.team.manage", ct))
            return ApiResponse<object?>.Fail(["You do not have permission to manage attendance staff."]);
        var entity = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(db.AttendanceStaffAssignments, x => x.Id == assignmentId && x.EventId == eventId && x.IsActive, ct);
        if (entity is null) return ApiResponse<object?>.Fail(["Attendance staff assignment not found."]);
        entity.IsActive = false; entity.RevokedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        // Drop the event role only when no other active assignment remains.
        var stillAssigned = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(
            db.AttendanceStaffAssignments,
            x => x.EventId == eventId && x.UserId == entity.UserId && x.IsActive, ct);
        if (!stillAssigned)
            await identity.RemoveEventRoleAsync(entity.UserId, eventId, AttendanceStaffRoleName, ct);

        return ApiResponse<object?>.Success(null, "Attendance staff assignment revoked.");
    }

    private static AttendanceStaffDto ToDto(AttendanceStaffAssignment x, string email) => new(x.Id,x.EventId,x.UserId,email,x.ScopeType,x.ScopeId,x.IsActive);
}
