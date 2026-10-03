using EventFlow.Registration.Domain.Entities;

namespace EventFlow.Registration.Application.Abstractions.Persistence;

public interface ICertificateSettingsRepository
{
    Task<CertificateSettings?> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    void Add(CertificateSettings settings);
}
