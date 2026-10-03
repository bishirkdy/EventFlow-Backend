using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Domain.Entities;

public sealed class Certificate
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public Guid RegistrationId { get; set; }

    public Guid ParticipantId { get; set; }

    public Guid UserId { get; set; }

    public string CertificateNumber { get; set; } = "";

    public string ParticipantName { get; set; } = "";

    public string ParticipantEmail { get; set; } = "";

    public string EventName { get; set; } = "";

    public string DocumentFileName { get; set; } = "";

    public CertificateStatus Status { get; set; } = CertificateStatus.Generated;

    public DateTime IssuedAtUtc { get; set; }

    public Guid GeneratedByUserId { get; set; }

    public DateTime? RevokedAtUtc { get; set; }
}
