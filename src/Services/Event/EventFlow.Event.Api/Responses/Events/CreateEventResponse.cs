using EventFlow.Event.Application.Features.Events.Common;

namespace EventFlow.Event.Api.Responses.Events
{
    public sealed record CreateEventResponse(
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
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        IReadOnlyList<EventImageResponse> Images);
}
