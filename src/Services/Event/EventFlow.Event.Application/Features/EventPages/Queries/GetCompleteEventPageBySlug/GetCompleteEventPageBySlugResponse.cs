

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetCompleteEventPageBySlug
{
    public sealed record GetCompleteEventPageBySlugResponse(
        Guid Id,
        Guid EventId,
        string Name,
        string Slug,
        string PageType,
        int DisplayOrder,
        bool IsPublished,
        IReadOnlyList<PageSectionResponse> Sections);

    public sealed record PageSectionResponse(
        Guid Id,
        Guid PageId,
        string SectionType,
        string? Title,
        string? Content,
        string? ImageUrl,
        int DisplayOrder,
        bool IsVisible,
        string? Configuration);
}
