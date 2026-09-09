

using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Commands.EnableEventFeature
{
    public sealed record EnableEventFeatureCommand(Guid EventId,Guid FeatureId) : IRequest;
}
