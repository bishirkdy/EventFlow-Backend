namespace EventFlow.Registration.Application.Abstractions.Services;

public interface IEventRegistrationAccessService
{
    Task<bool> IsRegistrationFeatureEnabledAsync(Guid eventId, CancellationToken ct = default);
    Task<bool> CanManageRegistrationAsync(Guid eventId, Guid userId, CancellationToken ct = default);
}
