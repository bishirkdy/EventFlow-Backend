using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetMyEvents
{
    // Query
    public sealed record GetMyEventsQuery(Guid UserId): IRequest<IReadOnlyList<GetMyEventsResponse>>;
}
