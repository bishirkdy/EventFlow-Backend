using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.UploadCertificateTemplateImage;

public sealed record UploadCertificateTemplateImageCommand(
    Guid EventId,
    UploadedFile? File
) : IRequest<CertificateSettingsDto>;
