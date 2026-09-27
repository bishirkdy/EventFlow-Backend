

using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.NavigationItemVisibility
{
    public sealed record SetNavigationItemVisibilityCommand(
        Guid EventId, Guid Id,
        bool IsVisible) : IRequest;
}
