using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Domain;

namespace EventFlow.Registration.Domain.Entities;

public sealed class CertificateSettings : Entity
{
    // EF Core materialization constructor.
    private CertificateSettings()
    {
    }

    public CertificateSettings(Guid eventId)
    {
        EventId = eventId;
    }

    public Guid EventId { get; private set; }

    public CertificateTemplateMode TemplateMode { get; private set; } = CertificateTemplateMode.Classic;

    public string? BackgroundImageUrl { get; private set; }

    public string? BackgroundImagePublicId { get; private set; }

    public int BackgroundImageWidth { get; private set; }

    public int BackgroundImageHeight { get; private set; }

    public List<CertificateTemplateField> TemplateFields { get; private set; } = [];

    public string Title { get; private set; } = "Certificate of Participation";

    public string Subtitle { get; private set; } = "";

    public string? SignatoryName { get; private set; }

    public string? SignatoryTitle { get; private set; }

    public string ThemeColor { get; private set; } = "#2563EB";

    public bool RequireApprovedRegistration { get; private set; } = true;

    public double? MinAttendancePercent { get; private set; }

    // Applies the editable certificate configuration.
    public void Update(
        string title,
        string subtitle,
        string? signatoryName,
        string? signatoryTitle,
        string themeColor,
        bool requireApprovedRegistration,
        double? minAttendancePercent,
        CertificateTemplateMode templateMode,
        List<CertificateTemplateField> templateFields)
    {
        Title = title;
        Subtitle = subtitle;
        SignatoryName = signatoryName;
        SignatoryTitle = signatoryTitle;
        ThemeColor = themeColor;
        RequireApprovedRegistration = requireApprovedRegistration;
        MinAttendancePercent = minAttendancePercent;
        TemplateMode = templateMode;
        TemplateFields = templateFields;
        SetUpdatedAt();
    }

    // Stores the uploaded background image details.
    public void SetBackgroundImage(string url, string publicId, int width, int height)
    {
        BackgroundImageUrl = url;
        BackgroundImagePublicId = publicId;
        BackgroundImageWidth = width;
        BackgroundImageHeight = height;
        SetUpdatedAt();
    }

    // Clears the background image details.
    public void ClearBackgroundImage()
    {
        BackgroundImageUrl = null;
        BackgroundImagePublicId = null;
        BackgroundImageWidth = 0;
        BackgroundImageHeight = 0;
        SetUpdatedAt();
    }
}
