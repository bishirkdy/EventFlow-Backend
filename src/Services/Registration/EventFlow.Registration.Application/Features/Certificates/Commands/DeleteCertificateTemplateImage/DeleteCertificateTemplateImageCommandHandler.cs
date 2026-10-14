using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using Microsoft.Extensions.Logging;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.DeleteCertificateTemplateImage;

public sealed class DeleteCertificateTemplateImageCommandHandler(
    ICertificateSettingsRepository settings,
    ICertificateTemplateImageStore imageStore,
    IUnitOfWork unitOfWork,
    ICurrentUserService user,
    IEventRegistrationAccessService access,
    ILogger<DeleteCertificateTemplateImageCommandHandler> logger)
    : IRequestHandler<DeleteCertificateTemplateImageCommand, CertificateSettingsDto>
{
    public async Task<CertificateSettingsDto> Handle(
        DeleteCertificateTemplateImageCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var entity = await settings.GetByEventIdAsync(
            command.EventId,
            cancellationToken);

        var publicId = entity?.BackgroundImagePublicId;

        if (entity is null || string.IsNullOrWhiteSpace(publicId))
        {
            throw new NotFoundException("No certificate template image is uploaded for this event.");
        }

        entity.BackgroundImageUrl = null;
        entity.BackgroundImagePublicId = null;
        entity.BackgroundImageWidth = 0;
        entity.BackgroundImageHeight = 0;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await imageStore.DeleteAsync(publicId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Certificate template image {PublicId} was removed from settings but could not be deleted from storage.",
                publicId);
        }

        return entity.ToSettingsDto();
    }
}
