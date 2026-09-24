using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Domain.Entities;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Commands.CreateVenue
{
    public sealed class CreateVenueCommandHandler(IVenueRepository venueRepository, IUnitOfWork unitOfWork, IFileStorage fileStorage)
        : IRequestHandler<CreateVenueCommand, Guid>
    {
        public async Task<Guid> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
        {
            string? imageUrl = null;
            if (request.Image is not null)
            {
                var stored = await fileStorage.SaveAsync(request.Image, $"events/{request.EventId:D}/venues", cancellationToken);
                imageUrl = stored.Url;
            }

            var venue = new Venue(request.EventId, request.Name, request.Description, request.Address, request.Capacity, imageUrl);
            await venueRepository.AddAsync(venue, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return venue.Id;
        }
    }
}
