

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetEventFeatures;

public sealed class GetEventFeaturesQueryHandler(IEventFeatureRepository eventFeatureRepository, IMapper mapper)
    : IRequestHandler<GetEventFeaturesQuery, IReadOnlyList<GetEventFeaturesResponse>>
{
    public async Task<IReadOnlyList<GetEventFeaturesResponse>> Handle(GetEventFeaturesQuery request, CancellationToken cancellationToken)
    {
        var features = await eventFeatureRepository.GetByEventIdAsync(request.EventId, cancellationToken);

        return mapper.Map<IReadOnlyList<GetEventFeaturesResponse>>(features);
    }
}
