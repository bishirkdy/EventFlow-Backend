using EventFlow.Operations.Domain.Enums;

namespace EventFlow.Operations.Domain.Entities;

public sealed class AttendanceStaffAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public AttendanceScopeType ScopeType { get; set; }
    public Guid? ScopeId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAtUtc { get; set; }
}
