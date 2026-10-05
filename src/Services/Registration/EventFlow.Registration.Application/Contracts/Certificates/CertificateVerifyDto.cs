namespace EventFlow.Registration.Application.Contracts.Certificates;

public sealed class CertificateVerifyDto
{
    public string CertificateNumber { get; set; } = "";
    public string ParticipantName { get; set; } = "";
    public string EventName { get; set; } = "";
    public string Status { get; set; } = "";
    public bool IsValid { get; set; }
    public DateTime IssuedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
}
