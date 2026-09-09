
namespace EventFlow.Event.Application.Features.Sections.Queries.GetSectionById
{
    public sealed record GetSectionByIdResponse(
        Guid Id,
        Guid EventId,
        string Name,
        string? Description,
        int DisplayOrder,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
