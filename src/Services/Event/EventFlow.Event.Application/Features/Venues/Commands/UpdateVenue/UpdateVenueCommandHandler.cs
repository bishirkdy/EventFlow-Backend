

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Commands.UpdateVenue
{
    public sealed class UpdateVenueCommandHandler(IVenueRepository venueRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateVenueCommand>
    {
        public async Task Handle(UpdateVenueCommand request, CancellationToken cancellationToken)
        {
            // Get venue
            var venue = await venueRepository.GetByIdAsync(request.Id,cancellationToken);

            // Verify venue belongs to event
            if (venue is null || venue.EventId != request.EventId)
                throw new NotFoundException("Venue not found.");

            // Update venue
            venue.Update(
                request.Name,
                request.Description,
                request.Address,
                request.Capacity);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
