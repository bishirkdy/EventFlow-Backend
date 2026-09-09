

namespace EventFlow.Event.Application.Features.Sections.Queries.GetSectionsByEvent
{
    public sealed record GetSectionsByEventResponse(
        Guid Id,
        Guid EventId,
        string Name,
        string? Description,
        int DisplayOrder,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
