

using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.DeleteNavigationItem
{
    public sealed record DeleteNavigationItemCommand(
    Guid Id,
    Guid EventId) : IRequest;
}
