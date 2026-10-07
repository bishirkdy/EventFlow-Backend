using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateAnalytics;

public sealed record GetCertificateAnalyticsQuery(Guid EventId): IRequest<GetCertificateAnalyticsResponse>;
 