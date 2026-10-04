using EventFlow.Registration.Domain.Entities;

namespace EventFlow.Registration.Application.Abstractions.Persistence;

public interface ICertificateRepository
{
    Task<IReadOnlyList<Certificate>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Certificate>> GetForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Certificate?> GetByIdAsync(
        Guid eventId,
        Guid certificateId,
        CancellationToken cancellationToken = default);

    Task<Certificate?> GetByNumberAsync(
        string certificateNumber,
        CancellationToken cancellationToken = default);

    Task<Certificate?> GetByRegistrationAsync(
        Guid eventId,
        Guid registrationId,
        CancellationToken cancellationToken = default);

    Task<Certificate?> GetByUserAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<HashSet<Guid>> GetIssuedRegistrationIdsAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    void Add(Certificate certificate);
}
