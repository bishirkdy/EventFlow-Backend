using EventFlow.Event.Domain.Enums;


namespace EventFlow.Event.Application.Features.EventType.Queries.GetActiveEventTypes
{
    public sealed record GetEventTypeResponse(
        Guid Id,
        EventTypeCode Code,
        string Name,
        string? Description);
}
