

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Queries.GetVenuesByEvent
{
    public sealed class GetVenuesByEventQueryHandler(IVenueRepository venueRepository,IMapper mapper)
        : IRequestHandler<GetVenuesByEventQuery,IReadOnlyList<GetVenuesByEventResponse>>
    {
        public async Task<IReadOnlyList<GetVenuesByEventResponse>> Handle(
            GetVenuesByEventQuery request,
            CancellationToken cancellationToken)
        {
            // Get venues belonging to event
            var venues = await venueRepository.GetByEventIdAsync(request.EventId,cancellationToken);

            // Map entities to responses
            return mapper.Map<IReadOnlyList<GetVenuesByEventResponse>>(venues);
        }
    }
}
