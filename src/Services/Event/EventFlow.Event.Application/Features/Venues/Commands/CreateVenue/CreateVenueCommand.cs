using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Commands.CreateVenue
{
    public sealed record CreateVenueCommand(
        Guid EventId,
        string Name,
        string? Description,
        string? Address,
        int Capacity) : IRequest<Guid>;
}
