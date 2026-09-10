

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Queries.GetPageSections
{
    public sealed class GetPageSectionsHandler(IPageSectionRepository pageSectionRepository,IMapper mapper)
        : IRequestHandler<GetPageSectionsQuery,IReadOnlyList<GetPageSectionsResponse>>
    {
        public async Task<IReadOnlyList<GetPageSectionsResponse>> Handle(GetPageSectionsQuery request,CancellationToken cancellationToken)
        {
            // Get sections
            var sections = await pageSectionRepository.GetByPageIdAsync(request.PageId,cancellationToken);
            return mapper.Map<IReadOnlyList<GetPageSectionsResponse>>(sections);
        }
    }
}
