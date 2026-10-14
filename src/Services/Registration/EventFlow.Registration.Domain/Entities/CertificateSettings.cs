using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Domain.Entities;

public sealed class CertificateSettings
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public CertificateTemplateMode TemplateMode { get; set; } = CertificateTemplateMode.Classic;

    public string? BackgroundImageUrl { get; set; }

    public string? BackgroundImagePublicId { get; set; }

    public int BackgroundImageWidth { get; set; }

    public int BackgroundImageHeight { get; set; }

    public List<CertificateTemplateField> TemplateFields { get; set; } = [];

    public string Title { get; set; } = "Certificate of Participation";

    public string Subtitle { get; set; } = "";

    public string? SignatoryName { get; set; }

    public string? SignatoryTitle { get; set; }

    public string ThemeColor { get; set; } = "#2563EB";

    public bool RequireApprovedRegistration { get; set; } = true;

    public double? MinAttendancePercent { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
