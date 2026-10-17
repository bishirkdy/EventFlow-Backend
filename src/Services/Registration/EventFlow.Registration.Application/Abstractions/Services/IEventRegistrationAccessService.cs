using EventFlow.Registration.Application.Contracts.Events;

namespace EventFlow.Registration.Application.Abstractions.Services;

public interface IEventRegistrationAccessService
{
    Task<bool> IsRegistrationFeatureEnabledAsync(Guid eventId, CancellationToken ct = default);
    Task<bool> IsEventPublishedAsync(Guid eventId, CancellationToken ct = default);
    Task<bool> CanPreviewEventAsync(Guid eventId, Guid userId, CancellationToken ct = default);
    Task<bool> CanManageRegistrationAsync(Guid eventId, Guid userId, CancellationToken ct = default);
    Task<SessionRegistrationInfo?> GetSessionRegistrationInfoAsync(Guid eventId, Guid sessionId, CancellationToken ct = default);
}
