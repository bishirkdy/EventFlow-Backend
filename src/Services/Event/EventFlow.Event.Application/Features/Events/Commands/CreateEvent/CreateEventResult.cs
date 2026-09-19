using EventFlow.Event.Application.Features.Events.Common;

namespace EventFlow.Event.Application.Features.Events.Commands.CreateEvent
{
    public sealed record CreateEventResult(
        Guid Id,
        string Name,
        string? Description,
        string EventType,
        string? SubType,
        DateTime StartDate,
        DateTime EndDate,
        string TimeZone,
        string Status,
        string? Subdomain,
        Guid CreatedBy,
        string? CreatedByName,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        IReadOnlyList<EventImageResponse> Images);
}
