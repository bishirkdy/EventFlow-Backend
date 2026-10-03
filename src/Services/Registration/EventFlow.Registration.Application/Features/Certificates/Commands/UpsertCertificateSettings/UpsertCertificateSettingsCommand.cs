using EventFlow.Contracts.Common;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.UpsertCertificateSettings;

public sealed record UpsertCertificateSettingsCommand(
    Guid EventId,
    UpsertCertificateSettingsRequest Request
) : IRequest<ApiResponse<CertificateSettingsDto>>;
