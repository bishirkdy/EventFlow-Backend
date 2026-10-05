using EventFlow.Registration.Application.Contracts.Certificates;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.VerifyCertificate;

public sealed record VerifyCertificateQuery(
    string CertificateNumber
) : IRequest<CertificateVerifyDto>;
