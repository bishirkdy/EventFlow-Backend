

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using MediatR;

namespace EventFlow.Event.Application.Features.Sections.Queries.GetSectionById;

public sealed class GetSectionByIdQueryHandler(ISectionRepository sectionRepository, IMapper mapper, IEventFeatureRepository featureRepository)
    : IRequestHandler<GetSectionByIdQuery, GetSectionByIdResponse?>
{
    public async Task<GetSectionByIdResponse?> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var section = await sectionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (section is null)
            return null;

        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, section.EventId, FeatureIds.Sessions, "sessions", cancellationToken);

        return mapper.Map<GetSectionByIdResponse>(section);
    }
}
