namespace EventFlow.Event.Api.Requests.PageSection
{
    public sealed record UpdatePageSectionRequest(
        string SectionType,
        string? Title,
        string? Content,
        string? ImageUrl,
        int DisplayOrder,
        bool IsVisible,
        string? Configuration);
}
