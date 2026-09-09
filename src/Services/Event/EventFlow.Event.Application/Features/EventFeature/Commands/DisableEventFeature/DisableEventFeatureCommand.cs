

using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Commands.DisableEventFeature
{
    public sealed record DisableEventFeatureCommand(Guid EventId,Guid FeatureId) : IRequest;
}
