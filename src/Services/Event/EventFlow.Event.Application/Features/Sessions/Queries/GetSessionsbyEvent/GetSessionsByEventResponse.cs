

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionsbyEvent
{
    public sealed record GetSessionsByEventResponse(
        Guid Id,
        Guid EventId,
        Guid SectionId,
        string Title,
        string? Description,
        string SessionType,
        int? Capacity,
        DateTime? StartTime,
        DateTime? EndTime,
        Guid? VenueId,
        string? ImageUrl,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
