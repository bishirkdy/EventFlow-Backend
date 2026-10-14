using EventFlow.Registration.Domain.Entities;

namespace EventFlow.Registration.Application.Abstractions.Services;

public sealed record CertificateDocumentData(
    string CertificateNumber,
    string ParticipantName,
    string EventName,
    string EventDatesText,
    string Title,
    string Subtitle,
    string ThemeColor,
    string? SignatoryName,
    string? SignatoryTitle,
    DateTime IssuedAtUtc,
    string VerificationUrl,
    bool UseImageTemplate = false,
    IReadOnlyList<CertificateTemplateField>? TemplateFields = null,
    byte[]? BackgroundImage = null,
    int BackgroundImageWidth = 0,
    int BackgroundImageHeight = 0);
