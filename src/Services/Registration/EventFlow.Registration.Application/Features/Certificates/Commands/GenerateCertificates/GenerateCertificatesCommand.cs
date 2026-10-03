using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Contracts.Certificates;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.GenerateCertificates;

public sealed record GenerateCertificatesCommand(
    Guid EventId,
    GenerateCertificatesRequest Request
) : IRequest<ApiResponse<CertificateGenerationResultDto>>;
