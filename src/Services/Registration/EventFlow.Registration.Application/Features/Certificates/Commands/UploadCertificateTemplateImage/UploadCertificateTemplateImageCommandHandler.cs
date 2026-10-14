using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using Microsoft.Extensions.Logging;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.UploadCertificateTemplateImage;

public sealed class UploadCertificateTemplateImageCommandHandler(
    ICertificateSettingsRepository settings,
    ICertificateTemplateImageStore imageStore,
    IUnitOfWork unitOfWork,
    ICurrentUserService user,
    IEventRegistrationAccessService access,
    ILogger<UploadCertificateTemplateImageCommandHandler> logger)
    : IRequestHandler<UploadCertificateTemplateImageCommand, CertificateSettingsDto>
{
    public async Task<CertificateSettingsDto> Handle(
        UploadCertificateTemplateImageCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var file = command.File!;

        var entity = await settings.GetByEventIdAsync(
            command.EventId,
            cancellationToken);

        var now = DateTime.UtcNow;

        if (entity is null)
        {
            entity = new CertificateSettings
            {
                Id = Guid.NewGuid(),
                EventId = command.EventId,
                CreatedAtUtc = now
            };
            settings.Add(entity);
        }

        var image = await imageStore.SaveAsync(
            command.EventId,
            file,
            cancellationToken);

        var previousPublicId = entity.BackgroundImagePublicId;

        entity.BackgroundImageUrl = image.Url;
        entity.BackgroundImagePublicId = image.PublicId;
        entity.BackgroundImageWidth = image.Width;
        entity.BackgroundImageHeight = image.Height;
        entity.UpdatedAtUtc = now;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await DeletePreviousImageAsync(previousPublicId, image.PublicId, cancellationToken);

        return entity.ToSettingsDto();
    }

    private async Task DeletePreviousImageAsync(
        string? previousPublicId,
        string currentPublicId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(previousPublicId) ||
            previousPublicId == currentPublicId)
        {
            return;
        }

        try
        {
            await imageStore.DeleteAsync(previousPublicId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Failed to delete previous certificate template image {PublicId}.",
                previousPublicId);
        }
    }
}
