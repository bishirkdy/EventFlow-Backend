using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationAnalytics;

public sealed record GetRegistrationAnalyticsQuery(
    Guid EventId,int Days = 30): IRequest<GetRegistrationAnalyticsResponse>;
