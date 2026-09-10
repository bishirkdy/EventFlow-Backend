namespace EventFlow.Event.Api.Requests.NavigationItems
{
    public sealed record UpdateNavigationItemRequest(
        string Label,
        string? Url,
        Guid? PageId,
        int DisplayOrder,
        bool OpenInNewTab,
        bool IsVisible);
}
