using MediatR;


namespace EventFlow.Event.Application.Features.Events.Commands.PublishEvent
{
    // Command
    public sealed record PublishEventCommand(Guid Id) : IRequest;
}
