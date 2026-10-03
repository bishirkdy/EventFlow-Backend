using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Features.PageSection.Queries.GetPageSections;
using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Queries.GetPageSectionsByEvent;

public sealed class GetPageSectionsByEventHandler(
    IPageSectionRepository pageSectionRepository, IMapper mapper)
    : IRequestHandler<GetPageSectionsByEventQuery, IReadOnlyList<GetPageSectionsResponse>>
{
    public async Task<IReadOnlyList<GetPageSectionsResponse>> Handle(
        GetPageSectionsByEventQuery request, CancellationToken cancellationToken)
    {
        var sections = await pageSectionRepository.GetByEventIdAsync(
            request.EventId, request.IncludeUnpublished, cancellationToken);

        return mapper.Map<IReadOnlyList<GetPageSectionsResponse>>(sections);
    }
}
