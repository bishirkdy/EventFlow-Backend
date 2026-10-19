using MediatR;

namespace EventFlow.Event.Application.Features.Commands.UpdateEventPhotosVisibility;

public sealed record UpdateEventPhotosVisibilityCommand(
    Guid EventId,
    IReadOnlyList<Guid> PhotoIds,
    bool IsVisible,
    Guid ApprovedBy) : IRequest<int>;
