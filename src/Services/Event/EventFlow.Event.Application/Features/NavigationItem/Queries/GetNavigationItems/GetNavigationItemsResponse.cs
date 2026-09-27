namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItems
{
    public sealed record GetNavigationItemsResponse(
        Guid Id,
        Guid EventId,
        string Label,
        Guid PageId,
        int DisplayOrder,
        bool IsVisible,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
