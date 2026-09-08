namespace EventFlow.Event.Api.Requests.Events
{
    public sealed record CreateEventRequest(
        string Name,
        string? Description,
        string EventType,
        string? SubType,
        DateTime StartDate,
        DateTime EndDate,
        string TimeZone);
}
