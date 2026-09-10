
using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.UpdateNavigationItem
{
    public sealed record UpdateNavigationItemCommand(
        Guid NavigationMenuId,
        Guid Id,
        string Label,
        string? Url,
        Guid? PageId,
        int DisplayOrder,
        bool OpenInNewTab,
        bool IsVisible) : IRequest;
}
