using EventFlow.Operations.Domain.Enums;
using EventFlow.SharedKernel.Domain;

namespace EventFlow.Operations.Domain.Entities;

public sealed class AttendanceStaffAssignment : Entity
{
    // EF Core materialization constructor.
    private AttendanceStaffAssignment()
    {
    }

    public AttendanceStaffAssignment(
        Guid eventId,
        Guid userId,
        AttendanceScopeType scopeType,
        Guid? scopeId)
    {
        EventId = eventId;
        UserId = userId;
        ScopeType = scopeType;
        ScopeId = scopeId;
        IsActive = true;
    }

    public Guid EventId { get; private set; }

    public Guid UserId { get; private set; }

    public AttendanceScopeType ScopeType { get; private set; }

    public Guid? ScopeId { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    // Revokes the assignment so the staff member loses access.
    public void Revoke()
    {
        IsActive = false;
        RevokedAtUtc = DateTime.UtcNow;
        SetUpdatedAt();
    }
}
