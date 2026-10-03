namespace EventFlow.Registration.Application.Abstractions.Services;

public interface ICertificateSourceDataService
{
    Task<CertificateEventInfo?> GetEventAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    Task<double?> GetAttendancePercentAsync(
        Guid eventId,
        Guid participantUserId,
        CancellationToken cancellationToken = default);
}

public sealed record CertificateEventInfo(
    Guid Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string TimeZone);
