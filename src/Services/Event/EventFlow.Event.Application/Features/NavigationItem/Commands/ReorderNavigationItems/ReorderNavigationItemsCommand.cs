

using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.ReorderNavigationItems
{
    public sealed record ReorderNavigationItemsCommand(Guid NavigationMenuId, IReadOnlyList<Guid> ItemIds) : IRequest;
}
