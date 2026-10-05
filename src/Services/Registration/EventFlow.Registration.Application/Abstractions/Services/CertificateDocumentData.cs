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
    string VerificationUrl);
