using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using MediatR;
namespace EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorsByEvent;
public sealed class GetSponsorsByEventQueryHandler(ISponsorRepository repository, IEventFeatureRepository featureRepository, IMapper mapper) : IRequestHandler<GetSponsorsByEventQuery, IReadOnlyList<GetSponsorsByEventResponse>>
{
    public async Task<IReadOnlyList<GetSponsorsByEventResponse>> Handle(GetSponsorsByEventQuery request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Sponsors, "sponsors", cancellationToken);
        return mapper.Map<IReadOnlyList<GetSponsorsByEventResponse>>(await repository.GetByEventIdAsync(request.EventId, cancellationToken));
    }
}
