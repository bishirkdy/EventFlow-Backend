using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetEventPreviewAccess;

public sealed record GetEventPreviewAccessQuery(Guid EventId, Guid UserId)
    : IRequest<bool>;
