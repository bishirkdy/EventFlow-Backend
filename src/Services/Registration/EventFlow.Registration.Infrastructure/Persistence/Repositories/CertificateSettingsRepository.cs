using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Domain.Entities;

namespace EventFlow.Registration.Infrastructure.Persistence.Repositories;

public sealed class CertificateSettingsRepository(RegistrationDbContext db)
    : ICertificateSettingsRepository
{
    public Task<CertificateSettings?> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        return db.CertificateSettings
            .SingleOrDefaultAsync(
                x => x.EventId == eventId,
                cancellationToken);
    }

    public void Add(CertificateSettings settings)
    {
        db.CertificateSettings.Add(settings);
    }
}
