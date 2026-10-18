using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using MediatR;

namespace EventFlow.Event.Application.Features.Sections.Queries.GetSectionsByEvent;

public sealed class GetSectionsByEventQueryHandler(ISectionRepository sectionRepository, IMapper mapper, IEventFeatureRepository featureRepository)
    : IRequestHandler<GetSectionsByEventQuery, IReadOnlyList<GetSectionsByEventResponse>>
{
    public async Task<IReadOnlyList<GetSectionsByEventResponse>> Handle(GetSectionsByEventQuery request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Sessions, "sessions", cancellationToken);

        var sections = await sectionRepository.GetByEventIdAsync(request.EventId, cancellationToken);

        return mapper.Map<IReadOnlyList<GetSectionsByEventResponse>>(sections);
    }
}
