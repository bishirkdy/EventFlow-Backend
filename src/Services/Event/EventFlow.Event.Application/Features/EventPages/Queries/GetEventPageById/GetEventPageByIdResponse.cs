namespace EventFlow.Event.Application.Features.EventPages.Queries.GetEventPageById
{
    public sealed record GetEventPageByIdResponse(
        Guid Id,
        Guid EventId,
        string Name,
        string Slug,
        string PageType,
        int DisplayOrder,
        bool IsPublished,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
