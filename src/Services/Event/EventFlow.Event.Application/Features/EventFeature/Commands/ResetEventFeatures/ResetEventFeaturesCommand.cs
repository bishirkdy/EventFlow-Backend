

using EventFlow.Event.Application.Features.EventFeature.Queries.GetEventFeatures;
using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Commands.ResetEventFeatures
{
    public sealed record ResetEventFeaturesCommand(Guid EventId) : IRequest<IReadOnlyList<GetEventFeaturesResponse>>;
}
