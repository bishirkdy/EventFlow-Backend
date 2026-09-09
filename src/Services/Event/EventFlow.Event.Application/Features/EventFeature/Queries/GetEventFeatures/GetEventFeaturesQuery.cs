

using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetEventFeatures
{
    public sealed record GetEventFeaturesQuery(Guid EventId): IRequest<IReadOnlyList<GetEventFeaturesResponse>>;
}
