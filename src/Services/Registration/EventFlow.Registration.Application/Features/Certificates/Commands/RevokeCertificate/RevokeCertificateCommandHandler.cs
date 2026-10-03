using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Domain.Enums;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.RevokeCertificate;

public sealed class RevokeCertificateCommandHandler(
    ICertificateRepository certificates,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<RevokeCertificateCommand, ApiResponse<CertificateDto>>
{
    public async Task<ApiResponse<CertificateDto>> Handle(
        RevokeCertificateCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<CertificateDto>.Fail(
                ["You do not have permission."]);
        }

        var certificate = await certificates.GetByIdAsync(
            command.EventId,
            command.CertificateId,
            cancellationToken);

        if (certificate is null)
        {
            return ApiResponse<CertificateDto>.Fail(["Certificate not found."]);
        }

        if (certificate.Status == CertificateStatus.Revoked)
        {
            return ApiResponse<CertificateDto>.Success(
                certificate.ToDto(),
                "Certificate is already revoked.");
        }

        certificate.Status = CertificateStatus.Revoked;
        certificate.RevokedAtUtc = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<CertificateDto>.Success(
            certificate.ToDto(),
            "Certificate revoked.");
    }
}
