using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;

using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Application.Abstractions.Persistence;

public interface IRegistrationRepository
{
    Task<RegistrationEntity?> GetByIdAsync(
        Guid eventId,
        Guid registrationId,
        Guid? userId = null,
        bool includeParticipant = false,
        bool includeTicket = false,
        bool includeAnswers = false,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RegistrationEntity>> GetForUserAsync(
        Guid userId,
        Guid? eventId = null,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<RegistrationEntity> Items, int TotalCount)> GetPagedForEventAsync(
        Guid eventId,
        RegistrationStatus? status,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> HasActiveRegistrationAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<int> CountByStatusAsync(
        Guid eventId,
        RegistrationStatus status,
        CancellationToken cancellationToken = default);

    Task<RegistrationStatisticsReadModel> GetStatisticsAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    void Add(RegistrationEntity registration);

    void RemoveAnswers(IEnumerable<RegistrationAnswer> answers);
}
