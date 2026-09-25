using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;

namespace EventFlow.Event.Application.Common;

public static class EventFeatureGuard
{
    public static async Task EnsureEnabledAsync(IEventFeatureRepository repository, Guid eventId, Guid featureId, string featureName, CancellationToken cancellationToken)
    {
        var feature = await repository.GetByEventAndFeatureAsync(eventId, featureId, cancellationToken);
        if (feature is null || !feature.IsEnabled)
            throw new NotFoundException($"The {featureName} feature is not enabled for this event.");
    }
}
