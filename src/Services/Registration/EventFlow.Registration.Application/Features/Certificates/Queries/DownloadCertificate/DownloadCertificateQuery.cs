using EventFlow.Registration.Application.Contracts.Certificates;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.DownloadCertificate;

public sealed record DownloadCertificateQuery(
    Guid EventId,
    Guid CertificateId
) : IRequest<CertificateDownloadDto>;
