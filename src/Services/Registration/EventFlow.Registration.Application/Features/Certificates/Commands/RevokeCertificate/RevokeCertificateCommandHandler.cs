using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.RevokeCertificate;

public sealed class RevokeCertificateCommandHandler(
    ICertificateRepository certificates,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<RevokeCertificateCommand, CertificateDto>
{
    public async Task<CertificateDto> Handle(
        RevokeCertificateCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var certificate = await certificates.GetByIdAsync(
            command.EventId,
            command.CertificateId,
            cancellationToken);

        if (certificate is null)
        {
            throw new NotFoundException("Certificate not found.");
        }

        if (certificate.Status == CertificateStatus.Revoked)
        {
            return certificate.ToDto();
        }

        certificate.Status = CertificateStatus.Revoked;
        certificate.RevokedAtUtc = DateTime.UtcNow;

        certificates.Update(certificate);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return certificate.ToDto();
    }
}
