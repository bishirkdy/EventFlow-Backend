using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.DeleteCertificateTemplateImage;

public sealed record DeleteCertificateTemplateImageCommand(
    Guid EventId
) : IRequest<CertificateSettingsDto>;
