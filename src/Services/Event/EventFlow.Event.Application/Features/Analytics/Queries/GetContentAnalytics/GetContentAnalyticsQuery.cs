using MediatR;

namespace EventFlow.Event.Application.Features.Analytics.Queries.GetContentAnalytics;

public sealed record GetContentAnalyticsQuery(Guid EventId)
    : IRequest<GetContentAnalyticsResponse>;
