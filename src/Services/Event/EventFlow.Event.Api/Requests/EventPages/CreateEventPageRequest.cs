namespace EventFlow.Event.Api.Requests.EventPages
{
    public sealed record CreateEventPageRequest(
        string Name,
        string Slug,
        string PageType,
        int DisplayOrder);
}
