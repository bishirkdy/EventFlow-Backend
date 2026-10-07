using MediatR;

namespace EventFlow.Event.Application.Features.Analytics.Queries.GetEventOverviewAnalytics;

public sealed record GetEventOverviewAnalyticsQuery(Guid EventId):  IRequest<GetEventOverviewAnalyticsResponse?>;
