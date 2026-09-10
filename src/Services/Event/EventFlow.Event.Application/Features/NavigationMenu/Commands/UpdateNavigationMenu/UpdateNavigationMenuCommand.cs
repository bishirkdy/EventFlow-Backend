

using MediatR;

namespace EventFlow.Event.Application.Features.NavigationMenu.Commands.UpdateNavigationMenu
{
    public sealed record UpdateNavigationMenuCommand(
        Guid EventId,
        Guid Id,
        string Name,
        string Location) : IRequest;
}
