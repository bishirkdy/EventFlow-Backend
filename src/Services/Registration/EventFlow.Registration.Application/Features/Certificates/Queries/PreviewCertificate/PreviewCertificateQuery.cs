using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.PreviewCertificate;

public sealed record PreviewCertificateQuery(
    Guid EventId,
    string? SampleName
) : IRequest<CertificateDownloadDto>;
