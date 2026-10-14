using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Application.Common.Mappings;

public static class CertificateSettingsMappings
{
    public static CertificateSettingsDto ToSettingsDto(this CertificateSettings entity) => new()
    {
        EventId = entity.EventId,
        Title = entity.Title,
        Subtitle = entity.Subtitle,
        SignatoryName = entity.SignatoryName,
        SignatoryTitle = entity.SignatoryTitle,
        ThemeColor = entity.ThemeColor,
        RequireApprovedRegistration = entity.RequireApprovedRegistration,
        MinAttendancePercent = entity.MinAttendancePercent,
        TemplateMode = entity.TemplateMode.ToString(),
        BackgroundImageUrl = entity.BackgroundImageUrl,
        BackgroundImageWidth = entity.BackgroundImageWidth,
        BackgroundImageHeight = entity.BackgroundImageHeight,
        TemplateFields = entity.TemplateFields.Select(ToDto).ToList(),
        UpdatedAtUtc = entity.UpdatedAtUtc
    };

    public static CertificateTemplateFieldDto ToDto(this CertificateTemplateField field) => new()
    {
        Key = field.Key,
        Text = field.Text,
        XPercent = field.XPercent,
        YPercent = field.YPercent,
        WidthPercent = field.WidthPercent,
        FontSizePt = field.FontSizePt,
        FontColor = field.FontColor,
        Bold = field.Bold,
        Align = field.Align.ToString(),
        FontFamily = field.FontFamily
    };

    public static CertificateTemplateField ToEntity(this CertificateTemplateFieldDto dto) => new()
    {
        Key = dto.Key.Trim(),
        Text = string.IsNullOrWhiteSpace(dto.Text) ? null : dto.Text.Trim(),
        XPercent = dto.XPercent,
        YPercent = dto.YPercent,
        WidthPercent = dto.WidthPercent,
        FontSizePt = dto.FontSizePt,
        FontColor = string.IsNullOrWhiteSpace(dto.FontColor) ? "#111827" : dto.FontColor.Trim(),
        Bold = dto.Bold,
        Align = ParseAlign(dto.Align),
        FontFamily = string.IsNullOrWhiteSpace(dto.FontFamily) ? null : dto.FontFamily.Trim()
    };

    public static CertificateTemplateMode ParseTemplateMode(string? value) =>
        Enum.TryParse<CertificateTemplateMode>(value, ignoreCase: true, out var mode)
            ? mode
            : CertificateTemplateMode.Classic;

    private static CertificateFieldAlign ParseAlign(string? value) =>
        Enum.TryParse<CertificateFieldAlign>(value, ignoreCase: true, out var align)
            ? align
            : CertificateFieldAlign.Center;
}
