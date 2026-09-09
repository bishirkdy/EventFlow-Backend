

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetEventFeatures
{
    public sealed record GetEventFeaturesResponse(
        Guid Id,
        Guid EventId,
        Guid FeatureId,
        bool IsEnabled);
}
