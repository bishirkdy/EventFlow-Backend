namespace EventFlow.Event.Api.Requests.PageSection
{
    public sealed record CreatePageSectionRequest(
        string SectionType,
        string? Title,
        string? Content,
        IFormFile? Image,
        int DisplayOrder,
        string? Configuration);
}
