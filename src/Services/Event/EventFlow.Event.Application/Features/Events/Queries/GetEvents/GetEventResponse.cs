using EventFlow.Event.Application.Features.Events.Common;
namespace EventFlow.Event.Application.Features.Events.Queries.GetEvents
{
    public sealed record GetEventResponse(
        Guid Id,
        string Name,
        string? Description,
        string EventType,
        string? SubType,
        DateTime StartDate,
        DateTime EndDate,
        string TimeZone,
        string Status,
        IReadOnlyList<EventImageResponse> Images
    );
}
