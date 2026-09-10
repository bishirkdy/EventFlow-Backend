using MediatR;

namespace EventFlow.Event.Application.Features.NavigationItem.Commands.CreateNavigationItem
{
    public sealed record CreateNavigationItemCommand(
        Guid NavigationMenuId,
        string Label,
        string? Url,
        Guid? PageId,
        int DisplayOrder,
        bool OpenInNewTab) : IRequest<Guid>;
}
