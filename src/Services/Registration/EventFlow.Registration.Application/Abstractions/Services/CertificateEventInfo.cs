namespace EventFlow.Registration.Application.Abstractions.Services;

public sealed record CertificateEventInfo(
    Guid Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string TimeZone);
