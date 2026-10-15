using AutoMapper;
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
    ILogger<UploadCertificateTemplateImageCommandHandler> logger,
    IMapper mapper)
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

        if (entity is null)
        {
            entity = new CertificateSettings(command.EventId);
            settings.Add(entity);
        }

        var image = await imageStore.SaveAsync(
            command.EventId,
            file,
            cancellationToken);

        var previousPublicId = entity.BackgroundImagePublicId;

        entity.SetBackgroundImage(
            image.Url,
            image.PublicId,
            image.Width,
            image.Height);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await DeletePreviousImageAsync(previousPublicId, image.PublicId, cancellationToken);

        return mapper.Map<CertificateSettingsDto>(entity);
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
