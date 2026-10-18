using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetMyEvents;

public sealed record GetMyEventsQuery(Guid UserId) : IRequest<IReadOnlyList<GetMyEventsResponse>>;
