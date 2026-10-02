using EventFlow.Operations.Domain.Enums;

namespace EventFlow.Operations.Domain.Entities;

public sealed class AttendanceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EventId { get; set; }
    public Guid RegistrationId { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid ParticipantUserId { get; set; }
    public Guid? SectionId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid StaffUserId { get; set; }
    public AttendanceMethod Method { get; set; }
    public DateTime CheckedInAtUtc { get; set; }
    public DateTime? CheckedOutAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
