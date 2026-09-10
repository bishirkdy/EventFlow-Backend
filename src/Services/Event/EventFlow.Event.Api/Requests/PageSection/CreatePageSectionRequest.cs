namespace EventFlow.Event.Api.Requests.PageSection
{
    public sealed record CreatePageSectionRequest(
        string SectionType,
        string? Title,
        string? Content,
        string? ImageUrl,
        int DisplayOrder,
        string? Configuration);
}
