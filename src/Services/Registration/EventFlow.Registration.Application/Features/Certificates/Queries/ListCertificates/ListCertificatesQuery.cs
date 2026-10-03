using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Contracts.Certificates;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.ListCertificates;

public sealed record ListCertificatesQuery(
    Guid EventId
) : IRequest<ApiResponse<List<CertificateDto>>>;
