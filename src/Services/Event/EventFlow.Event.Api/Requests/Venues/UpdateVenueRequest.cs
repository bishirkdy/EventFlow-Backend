

namespace EventFlow.Event.Api.Requests.Venues
{
    public sealed record UpdateVenueRequest(
        string Name,
        string? Description,
        string? Address,
        int Capacity);
}
    