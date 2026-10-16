using EventFlow.Event.Application.Features.Events.Common;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetPublicEvents;

public sealed record GetPublicEventsQuery(int Take = 3) : IRequest<IReadOnlyList<GetEventResponse>>;
