
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationMenu.Commands.DeleteNavigationMenu
{
    public sealed record DeleteNavigationMenuCommand(
        Guid EventId,
        Guid Id) : IRequest;
}
