using EventFlow.Registration.Application.Contracts.Certificates;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateEligibility;

public sealed record GetCertificateEligibilityQuery(
    Guid EventId
) : IRequest<CertificateEligibilityDto>;
