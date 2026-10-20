using MediatR;

namespace EventFlow.Event.Application.Features.Events.Queries.GetPublicEventStats;

public sealed record GetPublicEventStatsQuery : IRequest<GetPublicEventStatsResponse>;
