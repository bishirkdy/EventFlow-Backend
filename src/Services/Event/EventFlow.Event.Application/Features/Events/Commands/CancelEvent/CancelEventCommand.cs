using MediatR;


namespace EventFlow.Event.Application.Features.Events.Commands.CancelEvent
{
    // Command
    public sealed record CancelEventCommand(Guid Id) : IRequest;
}
