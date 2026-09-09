using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Commands.UpdateVenueCapacity
{
    public sealed record UpdateVenueCapacityCommand(Guid Id,Guid EventId,int Capacity) : IRequest;
}
