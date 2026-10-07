using MediatR;

namespace EventFlow.Event.Application.Features.Analytics.Queries.GetProgrammeAnalytics;

public sealed record GetProgrammeAnalyticsQuery(Guid EventId) : IRequest<GetProgrammeAnalyticsResponse>;
