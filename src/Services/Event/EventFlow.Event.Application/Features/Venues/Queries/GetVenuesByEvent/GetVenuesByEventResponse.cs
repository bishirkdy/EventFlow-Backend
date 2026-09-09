
namespace EventFlow.Event.Application.Features.Venues.Queries.GetVenuesByEvent
{
    public sealed record GetVenuesByEventResponse(
        Guid Id,
        Guid EventId,
        string Name,
        string? Description,
        string? Address,
        int Capacity,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
