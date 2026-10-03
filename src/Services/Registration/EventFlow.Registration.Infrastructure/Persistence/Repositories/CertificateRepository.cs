using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Domain.Entities;

namespace EventFlow.Registration.Infrastructure.Persistence.Repositories;

public sealed class CertificateRepository(RegistrationDbContext db)
    : ICertificateRepository
{
    public async Task<IReadOnlyList<Certificate>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        return await db.Certificates
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .OrderByDescending(x => x.IssuedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<Certificate?> GetByIdAsync(
        Guid eventId,
        Guid certificateId,
        CancellationToken cancellationToken = default)
    {
        return db.Certificates
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.EventId == eventId && x.Id == certificateId,
                cancellationToken);
    }

    public Task<Certificate?> GetByNumberAsync(
        string certificateNumber,
        CancellationToken cancellationToken = default)
    {
        return db.Certificates
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.CertificateNumber == certificateNumber,
                cancellationToken);
    }

    public Task<Certificate?> GetByRegistrationAsync(
        Guid eventId,
        Guid registrationId,
        CancellationToken cancellationToken = default)
    {
        return db.Certificates
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.EventId == eventId && x.RegistrationId == registrationId,
                cancellationToken);
    }

    public Task<Certificate?> GetByUserAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return db.Certificates
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.EventId == eventId && x.UserId == userId,
                cancellationToken);
    }

    public async Task<HashSet<Guid>> GetIssuedRegistrationIdsAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var ids = await db.Certificates
            .Where(x => x.EventId == eventId)
            .Select(x => x.RegistrationId)
            .ToListAsync(cancellationToken);

        return [.. ids];
    }

    public void Add(Certificate certificate)
    {
        db.Certificates.Add(certificate);
    }
}
