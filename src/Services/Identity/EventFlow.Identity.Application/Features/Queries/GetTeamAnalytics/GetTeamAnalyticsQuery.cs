using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetTeamAnalytics;

public sealed record GetTeamAnalyticsQuery(Guid EventId)
    : IRequest<GetTeamAnalyticsResponse>;
