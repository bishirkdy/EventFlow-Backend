using EventFlow.Event.Application.Features.Events.Common;

namespace EventFlow.Event.Application.Features.Events.Queries.GetMyEvents
{
    public sealed record GetMyEventsResponse(
        Guid Id,
        string Name,
        string? Description,
        string EventType,
        string? SubType,
        DateTime StartDate,
        DateTime EndDate,
        string TimeZone,
        string Status,
        IReadOnlyList<EventImageResponse> Images);
}
