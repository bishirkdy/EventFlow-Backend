

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Commands.UpdateVenueCapacity
{
    public sealed class UpdateVenueCapacityCommandHandler(IVenueRepository venueRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateVenueCapacityCommand>
    {
        public async Task Handle(UpdateVenueCapacityCommand request, CancellationToken cancellationToken)
        {
            // Get venue
            var venue = await venueRepository.GetByIdAsync(request.Id,cancellationToken);

            // Verify venue belongs to event
            if (venue is null || venue.EventId != request.EventId)
                throw new NotFoundException("Venue not found.");

            // Update capacity
            venue.UpdateCapacity(request.Capacity);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
