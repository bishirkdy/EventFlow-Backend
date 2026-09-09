using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Commands.CreateVenue
{
    public sealed class CreateVenueCommandHandler(IVenueRepository venueRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<CreateVenueCommand, Guid>
    {
        public async Task<Guid> Handle(CreateVenueCommand request,CancellationToken cancellationToken)
        {
            // Create venue
            var venue = new Venue(
                request.EventId,
                request.Name,
                request.Description,
                request.Address,
                request.Capacity);

            // Save venue
            await venueRepository.AddAsync(venue,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return venue.Id;
        }
    }
}
