using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Contracts.Certificates;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetMyCertificates;

public sealed record GetMyCertificatesQuery(
    Guid EventId
) : IRequest<ApiResponse<List<CertificateDto>>>;
