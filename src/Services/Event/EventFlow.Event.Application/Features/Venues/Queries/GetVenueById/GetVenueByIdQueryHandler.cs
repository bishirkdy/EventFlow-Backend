

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Queries.GetVenueById
{
    public sealed class GetVenueByIdQueryHandler(IVenueRepository venueRepository,IMapper mapper)
        : IRequestHandler<GetVenueByIdQuery, GetVenueByIdResponse?>
    {
        public async Task<GetVenueByIdResponse?> Handle(GetVenueByIdQuery request,CancellationToken cancellationToken)
        {
            // Get venue
            var venue = await venueRepository.GetByIdAsync(request.Id,cancellationToken);

            // Verify venue belongs to event
            if (venue is null || venue.EventId != request.EventId)
                return null;

            // Map entity to response
            return mapper.Map<GetVenueByIdResponse>(venue);
        }
    }
}
