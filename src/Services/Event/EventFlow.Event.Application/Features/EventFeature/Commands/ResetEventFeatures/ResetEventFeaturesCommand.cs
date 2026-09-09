

using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Commands.ResetEventFeatures
{
    public sealed record ResetEventFeaturesCommand(Guid EventId) : IRequest;
}
