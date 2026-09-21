namespace EventFlow.Event.Api.Requests.PageSection
{
    public sealed record UpdatePageSectionRequest(
        string SectionType,
        string? Title,
        string? Content,
        IFormFile? Image,
        int DisplayOrder,
        bool IsVisible,
        string? Configuration);
}
