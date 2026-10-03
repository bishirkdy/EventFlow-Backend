using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Domain.Enums;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.VerifyCertificate;

public sealed class VerifyCertificateQueryHandler(
    ICertificateRepository certificates)
    : IRequestHandler<VerifyCertificateQuery, ApiResponse<CertificateVerifyDto>>
{
    public async Task<ApiResponse<CertificateVerifyDto>> Handle(
        VerifyCertificateQuery query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.CertificateNumber))
        {
            return ApiResponse<CertificateVerifyDto>.Fail(
                ["Certificate number is required."]);
        }

        var certificate = await certificates.GetByNumberAsync(
            query.CertificateNumber.Trim(),
            cancellationToken);

        if (certificate is null)
        {
            return ApiResponse<CertificateVerifyDto>.Fail(
                ["Certificate not found."]);
        }

        return ApiResponse<CertificateVerifyDto>.Success(
            new CertificateVerifyDto
            {
                CertificateNumber = certificate.CertificateNumber,
                ParticipantName = certificate.ParticipantName,
                EventName = certificate.EventName,
                Status = certificate.Status.ToString(),
                IsValid = certificate.Status == CertificateStatus.Generated,
                IssuedAtUtc = certificate.IssuedAtUtc,
                RevokedAtUtc = certificate.RevokedAtUtc
            },
            "Certificate verified.");
    }
}
