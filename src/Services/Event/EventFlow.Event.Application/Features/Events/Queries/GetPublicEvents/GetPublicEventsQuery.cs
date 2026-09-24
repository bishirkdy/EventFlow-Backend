using EventFlow.Event.Application.Features.Events.Queries.GetEvents;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetPublicEvents;

public sealed record GetPublicEventsQuery(int Take = 3) : IRequest<IReadOnlyList<GetEventResponse>>;
