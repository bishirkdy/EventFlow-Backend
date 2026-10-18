using MediatR;

namespace EventFlow.Event.Application.Features.Admin.Queries.GetAdminEventStats;

public sealed record GetAdminEventStatsQuery : IRequest<GetAdminEventStatsResponse>;
