namespace EventFlow.Event.Application.Abstractions.Services;

public interface IEventWebsiteProvisioningService
{
    Task EnsureInitialWebsiteAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task EnsureResourcePageAsync(Guid eventId, string resourceType, CancellationToken cancellationToken = default);
}
