namespace EventFlow.Event.Api.Responses.Events
{
    public sealed record GetEventResponse(
        Guid Id,
        string Name,
        string? Description,
        string EventType,
        string? SubType,
        DateTime StartDate,
        DateTime EndDate,
        string Timezone,
        string Status,
        string? Subdomain,
        Guid CreatedBy,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
