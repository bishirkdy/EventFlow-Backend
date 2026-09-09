

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetEventPagesByEvent
{
    public sealed record GetEventPagesByEventResponse(
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
