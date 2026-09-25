using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using MediatR;
namespace EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakersByEvent;
public sealed class GetSpeakersByEventQueryHandler(ISpeakerRepository repository, IEventFeatureRepository featureRepository, IMapper mapper) : IRequestHandler<GetSpeakersByEventQuery, IReadOnlyList<GetSpeakersByEventResponse>>
{
    public async Task<IReadOnlyList<GetSpeakersByEventResponse>> Handle(GetSpeakersByEventQuery request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Speakers, "speakers", cancellationToken);
        return mapper.Map<IReadOnlyList<GetSpeakersByEventResponse>>(await repository.GetByEventIdAsync(request.EventId, cancellationToken));
    }
}
