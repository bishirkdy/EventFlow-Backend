

using MediatR;

namespace EventFlow.Event.Application.Features.Events.Commands.CreateEvent
{
    public sealed record CreateEventCommand(
        string Name,
        string? Description,
        string EventType,
        string? SubType,
        DateTime StartDate,
        DateTime EndDate,
        string TimeZone
    ) : IRequest<CreateEventResult>;
}
