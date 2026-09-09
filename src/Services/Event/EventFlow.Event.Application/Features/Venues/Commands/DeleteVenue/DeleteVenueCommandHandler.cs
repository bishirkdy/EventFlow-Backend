
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Commands.DeleteVenue
{
    public sealed class DeleteVenueCommandHandler(IVenueRepository venueRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteVenueCommand>
    {
        public async Task Handle(DeleteVenueCommand request, CancellationToken cancellationToken)
        {
            // Get venue
            var venue = await venueRepository.GetByIdAsync(request.Id, cancellationToken);

            // Verify venue belongs to event
            if (venue is null || venue.EventId != request.EventId)
                throw new NotFoundException("Venue not found.");

            // Deactivate venue
            venue.Deactivate();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
