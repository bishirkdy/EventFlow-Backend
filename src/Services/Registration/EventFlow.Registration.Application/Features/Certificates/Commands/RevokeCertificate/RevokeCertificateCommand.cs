using EventFlow.Registration.Application.Contracts.Certificates;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.RevokeCertificate;

public sealed record RevokeCertificateCommand(
    Guid EventId,
    Guid CertificateId
) : IRequest<CertificateDto>;
