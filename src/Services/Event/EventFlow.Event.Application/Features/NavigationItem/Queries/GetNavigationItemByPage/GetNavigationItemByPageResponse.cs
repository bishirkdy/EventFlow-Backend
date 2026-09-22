

namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItemByPage
{
    public sealed record GetNavigationItemByPageResponse(
        Guid Id,
        Guid NavigationMenuId,
        string NavigationMenuName,
        string Label,
        Guid PageId,
        int DisplayOrder,
        bool IsVisible,
        bool OpenInNewTab);
}
