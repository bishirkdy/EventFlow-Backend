using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.UpdateNavigationItem
{
    public sealed record UpdateNavigationItemCommand(
        Guid Id,
        Guid EventId,
        string Label,
        Guid PageId,
        bool IsVisible) : IRequest;
}
