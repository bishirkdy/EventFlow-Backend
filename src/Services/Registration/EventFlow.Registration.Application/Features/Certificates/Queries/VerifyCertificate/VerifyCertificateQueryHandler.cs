using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.VerifyCertificate;

public sealed class VerifyCertificateQueryHandler(
    ICertificateRepository certificates)
    : IRequestHandler<VerifyCertificateQuery, CertificateVerifyDto>
{
    public async Task<CertificateVerifyDto> Handle(
        VerifyCertificateQuery query,
        CancellationToken cancellationToken)
    {
        var certificate = await certificates.GetByNumberAsync(
            query.CertificateNumber.Trim(),
            cancellationToken);

        if (certificate is null)
        {
            throw new NotFoundException("Certificate not found.");
        }

        return new CertificateVerifyDto
        {
            CertificateNumber = certificate.CertificateNumber,
            ParticipantName = certificate.ParticipantName,
            EventName = certificate.EventName,
            Status = certificate.Status.ToString(),
            IsValid = certificate.Status == CertificateStatus.Generated,
            IssuedAtUtc = certificate.IssuedAtUtc,
            RevokedAtUtc = certificate.RevokedAtUtc
        };
    }
}
