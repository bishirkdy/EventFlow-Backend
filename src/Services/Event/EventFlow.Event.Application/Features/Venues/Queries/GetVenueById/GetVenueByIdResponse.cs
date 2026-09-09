

namespace EventFlow.Event.Application.Features.Venues.Queries.GetVenueById
{
    public sealed record GetVenueByIdResponse(
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
