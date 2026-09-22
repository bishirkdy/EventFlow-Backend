

using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItemByPage
{
    public sealed record GetNavigationItemByPageQuery(Guid PageId) : IRequest<GetNavigationItemByPageResponse?>;
}
