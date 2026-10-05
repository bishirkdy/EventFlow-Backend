using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Queries.GetPageSections;

public sealed class GetPageSectionsQueryHandler(
    IPageSectionRepository pageSectionRepository,
    IMapper mapper)
    : IRequestHandler<GetPageSectionsQuery, IReadOnlyList<GetPageSectionsResponse>>
{
    public async Task<IReadOnlyList<GetPageSectionsResponse>> Handle(
        GetPageSectionsQuery request, CancellationToken cancellationToken)
    {
        var sections = await pageSectionRepository.GetByPageIdAsync(
            request.PageId, request.IncludeUnpublished, cancellationToken);

        return mapper.Map<IReadOnlyList<GetPageSectionsResponse>>(sections);
    }
}
