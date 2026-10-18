using MediatR;

namespace EventFlow.Event.Application.Features.Events.Commands.UpdateEvent;

public sealed record UpdateEventCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid EventTypeId,
    string? SubType,
    DateTime StartDate,
    DateTime EndDate,
    string TimeZone
) : IRequest;
