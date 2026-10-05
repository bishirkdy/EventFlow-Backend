namespace EventFlow.Registration.Application.Contracts.Certificates;

public sealed class CertificateSettingsDto
{
    public Guid EventId { get; set; }
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string? SignatoryName { get; set; }
    public string? SignatoryTitle { get; set; }
    public string ThemeColor { get; set; } = "";
    public bool RequireApprovedRegistration { get; set; }
    public double? MinAttendancePercent { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
