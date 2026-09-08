namespace EventFlow.Event.Api.Requests.Events
{
    public sealed record UpdateEventRequest(
        string Name,
        string? Description,
        string EventType,
        string? SubType,
        DateTime StartDate,
        DateTime EndDate,
        string TimeZone);
}
