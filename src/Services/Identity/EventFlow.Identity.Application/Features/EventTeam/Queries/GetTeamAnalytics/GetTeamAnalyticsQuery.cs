using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Queries.GetTeamAnalytics;

public sealed record GetTeamAnalyticsQuery(Guid EventId)
    : IRequest<GetTeamAnalyticsResponse>;
