namespace EventFlow.Event.Api.Requests.NavigationItems
{
    public sealed record UpdateNavigationItemRequest(
        string Label,
        Guid PageId,
        bool IsVisible);
}
