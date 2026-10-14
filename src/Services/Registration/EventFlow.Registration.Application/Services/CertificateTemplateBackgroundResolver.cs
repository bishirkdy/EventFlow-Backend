using EventFlow.SharedKernel.Exceptions;

namespace EventFlow.Registration.Application.Services;

public sealed class CertificateTemplateBackgroundResolver(
    ICertificateTemplateImageLoader imageLoader)
    : ICertificateTemplateBackgroundResolver
{
    public async Task<byte[]> ResolveAsync(
        CertificateSettings settings,
        CancellationToken cancellationToken = default)
    {
        if (settings.TemplateFields.Count == 0)
        {
            throw new ConflictException(
                "The image certificate template has no fields. Place the required fields on the certificate image first.");
        }

        if (string.IsNullOrWhiteSpace(settings.BackgroundImageUrl))
        {
            throw new ConflictException(
                "The image certificate template has no background image. Upload a certificate image first.");
        }

        var background = await imageLoader.LoadAsync(
            settings.BackgroundImageUrl,
            cancellationToken);

        if (background is null)
        {
            throw new ConflictException(
                "The certificate template image could not be loaded. Try again or upload the image again.");
        }

        return background;
    }
}
