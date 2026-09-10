
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItems
{
    public sealed record GetNavigationItemsQuery(Guid NavigationMenuId ) : IRequest<IReadOnlyList<GetNavigationItemsResponse>>;
}
