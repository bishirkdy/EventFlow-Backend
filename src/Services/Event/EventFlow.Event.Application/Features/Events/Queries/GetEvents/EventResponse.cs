using EventFlow.Event.Domain.Enums;


namespace EventFlow.Event.Application.Features.Events.Queries.GetEvents
{
    public sealed record EventResponse(
        Guid Id,
        string Name,
        string? Description,
        string EventType,
        string? SubType,
        DateTime StartDate,
        DateTime EndDate,
        string TimeZone,
        EventStatus Status
    );
}
