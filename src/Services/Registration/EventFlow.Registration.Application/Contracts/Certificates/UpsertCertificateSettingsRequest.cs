namespace EventFlow.Registration.Application.Contracts.Certificates;

public sealed class UpsertCertificateSettingsRequest
{
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string? SignatoryName { get; set; }
    public string? SignatoryTitle { get; set; }
    public string ThemeColor { get; set; } = "#2563EB";
    public bool RequireApprovedRegistration { get; set; } = true;
    public double? MinAttendancePercent { get; set; }
}
