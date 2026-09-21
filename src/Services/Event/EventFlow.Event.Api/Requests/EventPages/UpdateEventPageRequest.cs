namespace EventFlow.Event.Api.Requests.EventPages
{
    public sealed record UpdateEventPageRequest(
        string Name,
        string Slug,
        string PageType,
        int DisplayOrder);
}
