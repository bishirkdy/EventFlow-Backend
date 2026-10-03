using EventFlow.Contracts.Common;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateEligibility;

public sealed record GetCertificateEligibilityQuery(
    Guid EventId
) : IRequest<ApiResponse<CertificateEligibilityDto>>;
