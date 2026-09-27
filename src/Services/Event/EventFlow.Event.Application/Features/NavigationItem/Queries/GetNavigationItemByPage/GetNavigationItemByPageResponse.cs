namespace EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItemByPage
{
    public sealed record GetNavigationItemByPageResponse(
        Guid Id,
        Guid EventId,
        string Label,
        Guid PageId,
        int DisplayOrder,
        bool IsVisible);
}
