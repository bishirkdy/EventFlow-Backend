using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetCertificateSettings;

public sealed record GetCertificateSettingsQuery(
    Guid EventId
) : IRequest<CertificateSettingsDto>;
