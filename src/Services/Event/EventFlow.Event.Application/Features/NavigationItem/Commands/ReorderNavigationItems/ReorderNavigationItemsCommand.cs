

using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.ReorderNavigationItems
{
    public sealed record ReorderNavigationItemsCommand(
    Guid EventId,
    IReadOnlyList<Guid> ItemIds) : IRequest;
}
