

using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.NavigationItemVisibility
{
    public sealed record SetNavigationItemVisibilityCommand(
        Guid NavigationMenuId,Guid Id,
        bool IsVisible) : IRequest;
}
