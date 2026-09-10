namespace EventFlow.Event.Api.Requests.NavigationItem
{
    public sealed record CreateNavigationItemRequest(
        string Label,
        string? Url,
        Guid? PageId,
        int DisplayOrder,
        bool OpenInNewTab);
}
