namespace EventFlow.Registration.Application.Contracts.Certificates;

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
