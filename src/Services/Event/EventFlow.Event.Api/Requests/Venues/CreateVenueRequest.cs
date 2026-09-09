namespace EventFlow.Event.Api.Requests.Venues
{
    public sealed record CreateVenueRequest(
        string Name,
        string? Description,
        string? Address,
        int Capacity);
}
