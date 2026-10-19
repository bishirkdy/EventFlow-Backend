using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Application.Abstractions.Persistence;

public interface IRegistrationFormRepository
{
    Task<RegistrationForm?> GetByEventIdAsync(
        Guid eventId,
        bool includeFields = false,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    Task<RegistrationForm?> GetByScopeAsync(
        Guid eventId,
        RegistrationScope scopeType,
        Guid? scopeId,
        bool includeFields = false,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    // Returns the form configured for the scope, falling back to the event
    // form when a session has no dedicated form of its own.
    Task<RegistrationForm?> GetEffectiveByScopeAsync(
        Guid eventId,
        RegistrationScope scopeType,
        Guid? scopeId,
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
