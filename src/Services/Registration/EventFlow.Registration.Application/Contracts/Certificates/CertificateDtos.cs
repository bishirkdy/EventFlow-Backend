namespace EventFlow.Registration.Application.Contracts.Certificates;

public sealed class CertificateDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid RegistrationId { get; set; }
    public string CertificateNumber { get; set; } = "";
    public string ParticipantName { get; set; } = "";
    public string ParticipantEmail { get; set; } = "";
    public string EventName { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime IssuedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
}
