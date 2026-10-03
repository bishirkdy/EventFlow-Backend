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

public sealed class GenerateCertificatesRequest
{
    public List<Guid>? RegistrationIds { get; set; }
}

public sealed class CertificateGenerationResultDto
{
    public int Generated { get; set; }
    public int Skipped { get; set; }
    public int Failed { get; set; }
    public List<CertificateDto> Certificates { get; set; } = [];
}

public sealed class CertificateDownloadDto
{
    public string CertificateNumber { get; set; } = "";
    public string FileName { get; set; } = "";
    public string ContentType { get; set; } = "application/pdf";
    public byte[] Content { get; set; } = [];
}

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
