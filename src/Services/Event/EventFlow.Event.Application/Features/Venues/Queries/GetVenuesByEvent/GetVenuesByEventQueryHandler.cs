

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Queries.GetVenuesByEvent;

public sealed class GetVenuesByEventQueryHandler(IVenueRepository venueRepository, IMapper mapper, IEventFeatureRepository featureRepository)
    : IRequestHandler<GetVenuesByEventQuery, IReadOnlyList<GetVenuesByEventResponse>>
{
    public async Task<IReadOnlyList<GetVenuesByEventResponse>> Handle(
        GetVenuesByEventQuery request,
        CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Venues, "venues", cancellationToken);

        var venues = await venueRepository.GetByEventIdAsync(request.EventId, cancellationToken);

        return mapper.Map<IReadOnlyList<GetVenuesByEventResponse>>(venues);
    }
}
