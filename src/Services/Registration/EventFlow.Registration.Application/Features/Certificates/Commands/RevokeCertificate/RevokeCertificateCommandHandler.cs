using EventFlow.SharedKernel.Exceptions;
using EventFlow.Security.Authentication;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.RevokeCertificate;

public sealed class RevokeCertificateCommandHandler(
    ICertificateRepository certificates,
    IUnitOfWork unitOfWork,
    ICurrentUserService user,
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
