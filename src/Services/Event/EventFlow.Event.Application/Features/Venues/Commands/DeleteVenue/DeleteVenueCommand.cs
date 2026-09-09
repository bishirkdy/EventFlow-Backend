

using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Commands.DeleteVenue
{
    public sealed record DeleteVenueCommand(Guid Id, Guid EventId) : IRequest;
}
