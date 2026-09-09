

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionById
{
    public sealed record GetSessionByIdResponse(
        Guid Id,
        Guid EventId,
        Guid SectionId,
        string Title,
        string? Description,
        string SessionType,
        int? Capacity,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
