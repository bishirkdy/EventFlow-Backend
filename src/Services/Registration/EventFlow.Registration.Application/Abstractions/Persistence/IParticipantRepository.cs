using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Application.Abstractions.Persistence;

public interface IParticipantRepository
{
    Task<Participant?> GetByIdAsync(
        Guid eventId,
        Guid participantId,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Participant> Items, int TotalCount)> GetPagedForEventAsync(
        Guid eventId,
        ParticipantStatus? status,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
