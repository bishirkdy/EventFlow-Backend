namespace EventFlow.Event.Application.Features.Events.Queries.GetMyEvents
{
    // Query Response
    public sealed record GetMyEventsResponse(
        Guid Id,
        string Name,
        string? Description,
        string EventType,
        string? SubType,
        DateTime StartDate,
        DateTime EndDate,
        string TimeZone,
        string Status);
}
