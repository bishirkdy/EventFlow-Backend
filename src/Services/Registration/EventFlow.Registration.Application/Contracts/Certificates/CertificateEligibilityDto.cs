namespace EventFlow.Registration.Application.Contracts.Certificates;

public sealed class CertificateEligibilityDto
{
    public int TotalRegistrations { get; set; }
    public int EligibleCount { get; set; }
    public int AlreadyIssuedCount { get; set; }
    public int IneligibleCount { get; set; }
    public List<CertificateEligibilityItemDto> Items { get; set; } = [];
}

public sealed class CertificateEligibilityItemDto
{
    public Guid RegistrationId { get; set; }
    public Guid UserId { get; set; }
    public string RegistrationNumber { get; set; } = "";
    public string ParticipantName { get; set; } = "";
    public string Email { get; set; } = "";
    public string RegistrationStatus { get; set; } = "";
    public double? AttendancePercent { get; set; }
    public bool AlreadyIssued { get; set; }
    public bool Eligible { get; set; }
    public List<string> Reasons { get; set; } = [];
}
