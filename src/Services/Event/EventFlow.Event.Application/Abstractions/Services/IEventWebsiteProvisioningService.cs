namespace EventFlow.Event.Application.Abstractions.Services;

public interface IEventWebsiteProvisioningService
{
    Task EnsureInitialWebsiteAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task EnsureResourcePageAsync(Guid eventId, string resourceType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Makes sure the website page that belongs to a feature exists
    /// as soon as the feature is switched on for an event.
    /// </summary>
    Task EnsureFeaturePageAsync(Guid eventId, string featureCode, CancellationToken cancellationToken = default);
}
