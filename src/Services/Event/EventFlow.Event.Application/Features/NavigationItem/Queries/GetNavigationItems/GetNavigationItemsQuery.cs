
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItems
{
    public sealed record GetNavigationItemsQuery(Guid EventId, bool IncludeUnpublished = false) : IRequest<IReadOnlyList<GetNavigationItemsResponse>>;
}
