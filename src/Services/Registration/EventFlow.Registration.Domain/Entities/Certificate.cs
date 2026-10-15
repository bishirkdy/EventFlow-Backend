using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Domain;

namespace EventFlow.Registration.Domain.Entities;

public sealed class Certificate : Entity
{
    // EF Core materialization constructor.
    private Certificate()
    {
    }

    public Certificate(
        Guid eventId,
        Guid registrationId,
        Guid participantId,
        Guid userId,
        string certificateNumber,
        string participantName,
        string participantEmail,
        string eventName,
        string documentFileName,
        Guid generatedByUserId)
    {
        EventId = eventId;
        RegistrationId = registrationId;
        ParticipantId = participantId;
        UserId = userId;
        CertificateNumber = certificateNumber;
        ParticipantName = participantName;
        ParticipantEmail = participantEmail;
        EventName = eventName;
        DocumentFileName = documentFileName;
        GeneratedByUserId = generatedByUserId;
        Status = CertificateStatus.Generated;
        IssuedAtUtc = DateTime.UtcNow;
    }

    public Guid EventId { get; private set; }

    public Guid RegistrationId { get; private set; }

    public Guid ParticipantId { get; private set; }

    public Guid UserId { get; private set; }

    public string CertificateNumber { get; private set; } = "";

    public string ParticipantName { get; private set; } = "";

    public string ParticipantEmail { get; private set; } = "";

    public string EventName { get; private set; } = "";

    public string DocumentFileName { get; private set; } = "";

    public CertificateStatus Status { get; private set; } = CertificateStatus.Generated;

    public DateTime IssuedAtUtc { get; private set; }

    public Guid GeneratedByUserId { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    // Revokes the certificate so it can no longer be downloaded or verified.
    public void Revoke()
    {
        Status = CertificateStatus.Revoked;
        RevokedAtUtc = DateTime.UtcNow;
        SetUpdatedAt();
    }
}
