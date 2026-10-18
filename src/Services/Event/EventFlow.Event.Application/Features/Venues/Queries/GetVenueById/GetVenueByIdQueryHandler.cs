

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Queries.GetVenueById;

public sealed class GetVenueByIdQueryHandler(IVenueRepository venueRepository, IMapper mapper, IEventFeatureRepository featureRepository)
    : IRequestHandler<GetVenueByIdQuery, GetVenueByIdResponse?>
{
    public async Task<GetVenueByIdResponse?> Handle(GetVenueByIdQuery request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Venues, "venues", cancellationToken);

        var venue = await venueRepository.GetByIdAsync(request.Id, cancellationToken);

        if (venue is null || venue.EventId != request.EventId)
            return null;

        return mapper.Map<GetVenueByIdResponse>(venue);
    }
}
