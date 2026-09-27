namespace EventFlow.Event.Api.Requests.NavigationItem
{
    public sealed record CreateNavigationItemRequest(
        string Label,
        Guid PageId);
}
