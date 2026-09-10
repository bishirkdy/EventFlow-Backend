
using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItems
{
    public sealed class GetNavigationItemsHandler(INavigationItemRepository navigationItemRepository,IMapper mapper)
        : IRequestHandler<GetNavigationItemsQuery, IReadOnlyList<GetNavigationItemsResponse>>
    {
        public async Task<IReadOnlyList<GetNavigationItemsResponse>> Handle(GetNavigationItemsQuery request, CancellationToken cancellationToken)
        {
            // Get navigation items
            var items = await navigationItemRepository.GetByMenuIdAsync(request.NavigationMenuId,cancellationToken);

            // Map entities to responses
            return mapper.Map<IReadOnlyList<GetNavigationItemsResponse>>(items);
        }
    }
}
