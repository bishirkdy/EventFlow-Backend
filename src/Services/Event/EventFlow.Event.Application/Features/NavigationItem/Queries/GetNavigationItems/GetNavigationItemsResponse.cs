

namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItems
{
    public sealed record GetNavigationItemsResponse(
        Guid Id,
        Guid NavigationMenuId,
        string Label,
        string? Url,
        Guid? PageId,
        int DisplayOrder,
        bool IsVisible,
        bool OpenInNewTab,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
