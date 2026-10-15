using EventFlow.Operations.Domain.Enums;
using EventFlow.SharedKernel.Domain;

namespace EventFlow.Operations.Domain.Entities;

public sealed class AttendanceRecord : Entity
{
    // EF Core materialization constructor.
    private AttendanceRecord()
    {
    }

    public AttendanceRecord(
        Guid eventId,
        Guid registrationId,
        Guid participantId,
        Guid participantUserId,
        Guid? sectionId,
        Guid? sessionId,
        Guid staffUserId,
        AttendanceMethod method)
    {
        EventId = eventId;
        RegistrationId = registrationId;
        ParticipantId = participantId;
        ParticipantUserId = participantUserId;
        SectionId = sectionId;
        SessionId = sessionId;
        StaffUserId = staffUserId;
        Method = method;
        CheckedInAtUtc = DateTime.UtcNow;
    }

    public Guid EventId { get; private set; }

    public Guid RegistrationId { get; private set; }

    public Guid ParticipantId { get; private set; }

    public Guid ParticipantUserId { get; private set; }

    public Guid? SectionId { get; private set; }

    public Guid? SessionId { get; private set; }

    public Guid StaffUserId { get; private set; }

    public AttendanceMethod Method { get; private set; }

    public DateTime CheckedInAtUtc { get; private set; }

    public DateTime? CheckedOutAtUtc { get; private set; }

    // Marks the participant as checked out.
    public void CheckOut()
    {
        if (CheckedOutAtUtc is not null)
        {
            return;
        }

        CheckedOutAtUtc = DateTime.UtcNow;
        SetUpdatedAt();
    }
}
