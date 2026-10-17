using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetEventIsPublished;

public sealed record GetEventIsPublishedQuery(Guid EventId) : IRequest<bool>;
