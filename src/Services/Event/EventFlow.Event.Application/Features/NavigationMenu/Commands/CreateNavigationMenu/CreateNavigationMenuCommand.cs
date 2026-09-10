
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationMenu.Commands.CreateNavigationMenu
{
    public sealed record CreateNavigationMenuCommand(Guid EventId,string Name,string Location) : IRequest<Guid>;
}
