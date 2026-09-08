using MediatR;


namespace EventFlow.Event.Application.Features.Events.Commands.UpdateEvent
{
    //Command for update event
    public sealed record UpdateEventCommand(
        Guid Id,
        string Name,
        string? Description,
        string EventType,
        string? SubType,
        DateTime StartDate,
        DateTime EndDate,
        string TimeZone
    ) : IRequest;
}
