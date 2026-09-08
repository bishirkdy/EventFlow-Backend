

namespace EventFlow.Event.Application.Features.Events.Queries.GetEventById
{
    public sealed record GetEventByIdResponse(
        Guid Id,
        string Name,
        string? Description,
        string EventType,
        string? SubType,
        DateTime StartDate,
        DateTime EndDate,
        string EventTimeZone,
        string Status,
        string? Subdomain,
        Guid CreatedBy,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
