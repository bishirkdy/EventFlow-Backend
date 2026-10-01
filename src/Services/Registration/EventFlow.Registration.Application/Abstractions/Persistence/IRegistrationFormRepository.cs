using EventFlow.Registration.Domain.Entities;

namespace EventFlow.Registration.Application.Abstractions.Persistence;

public interface IRegistrationFormRepository
{
    Task<RegistrationForm?> GetByEventIdAsync(
        Guid eventId,
        bool includeFields = false,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlySet<Guid>> GetFieldIdsWithAnswersAsync(
        IReadOnlyCollection<Guid> fieldIds,
        CancellationToken cancellationToken = default);

    void Add(RegistrationForm form);

    void ReplaceFields(
        RegistrationForm form,
        IEnumerable<RegistrationFormField> fields);
}
