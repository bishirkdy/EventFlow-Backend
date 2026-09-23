

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetEventFeatures
{
    public sealed record GetEventFeaturesResponse(
        Guid Id,
        Guid EventId,
        Guid FeatureId,
        string FeatureCode,
        string FeatureName,
        string? FeatureDescription,
        bool IsEnabled);
}
