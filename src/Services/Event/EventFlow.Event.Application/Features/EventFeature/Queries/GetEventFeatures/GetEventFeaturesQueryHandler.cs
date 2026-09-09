

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetEventFeatures
{
    public sealed class GetEventFeaturesQueryHandler(IEventFeatureRepository eventFeatureRepository,IMapper mapper)
        : IRequestHandler<GetEventFeaturesQuery, IReadOnlyList<GetEventFeaturesResponse>>
    {
        public async Task<IReadOnlyList<GetEventFeaturesResponse>> Handle(GetEventFeaturesQuery request,CancellationToken cancellationToken)
        {
            // Get event features
            var features = await eventFeatureRepository.GetByEventIdAsync(request.EventId,cancellationToken);

            // Map entities to responses
            return mapper.Map<IReadOnlyList<GetEventFeaturesResponse>>(features);
        }
    }
}
