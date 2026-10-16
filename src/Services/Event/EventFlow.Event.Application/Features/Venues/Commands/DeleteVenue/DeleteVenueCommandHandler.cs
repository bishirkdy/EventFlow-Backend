
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Commands.DeleteVenue
{
    public sealed class DeleteVenueCommandHandler(IVenueRepository venueRepository, IUnitOfWork unitOfWork, IEventFeatureRepository featureRepository)
        : IRequestHandler<DeleteVenueCommand>
    {
        public async Task Handle(DeleteVenueCommand request, CancellationToken cancellationToken)
        {
            await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Venues, "venues", cancellationToken);

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
