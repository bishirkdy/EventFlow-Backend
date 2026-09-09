

using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionsbyEvent
{
    public sealed record GetSessionsByEventQuery(Guid EventId): IRequest<IReadOnlyList<GetSessionsByEventResponse>>;
}
