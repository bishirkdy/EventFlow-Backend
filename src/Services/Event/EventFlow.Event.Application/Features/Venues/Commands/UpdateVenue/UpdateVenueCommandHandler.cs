using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Commands.UpdateVenue
{
    public sealed class UpdateVenueCommandHandler(IVenueRepository venueRepository, IUnitOfWork unitOfWork, IFileStorage fileStorage)
        : IRequestHandler<UpdateVenueCommand>
    {
        public async Task Handle(UpdateVenueCommand request, CancellationToken cancellationToken)
        {
            var venue = await venueRepository.GetByIdAsync(request.Id, cancellationToken);
            if (venue is null || venue.EventId != request.EventId) throw new NotFoundException("Venue not found.");

            var imageUrl = venue.ImageUrl;
            if (request.Image is not null)
            {
                var stored = await fileStorage.SaveAsync(request.Image, $"events/{request.EventId:D}/venues", cancellationToken);
                imageUrl = stored.Url;
            }

            venue.Update(request.Name, request.Description, request.Address, request.Capacity, imageUrl);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
