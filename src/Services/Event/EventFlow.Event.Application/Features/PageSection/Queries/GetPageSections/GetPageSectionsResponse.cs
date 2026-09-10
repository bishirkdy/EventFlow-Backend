

namespace EventFlow.Event.Application.Features.PageSection.Queries.GetPageSections
{
    public sealed record GetPageSectionsResponse(
        Guid Id,
        Guid PageId,
        string SectionType,
        string? Title,
        string? Content,
        string? ImageUrl,
        int DisplayOrder,
        bool IsVisible,
        string? Configuration,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
