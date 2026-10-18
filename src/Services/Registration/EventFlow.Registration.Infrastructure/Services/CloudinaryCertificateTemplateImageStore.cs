using EventFlow.Infrastructure.Storage.Cloudinary;
using EventFlow.Registration.Application.Abstractions.Services;
using FluentValidation;

namespace EventFlow.Registration.Infrastructure.Services;

public sealed class CloudinaryCertificateTemplateImageStore(ICloudinaryStorage storage)
    : ICertificateTemplateImageStore
{
    private const long MaxBytes = 10 * 1024 * 1024;
    private const int MaxDimension = 20000;

    private static readonly string[] AllowedContentTypes =
    [
        "image/png",
        "image/jpeg",
        "image/jpg",
        "image/webp"
    ];

    public async Task<CertificateTemplateImage> SaveAsync(
        Guid eventId,
        UploadedFile file,
        CancellationToken cancellationToken = default)
    {
        if (file.Length <= 0 || file.Length > MaxBytes)
        {
            throw new ValidationException("The certificate image must be 10 MB or smaller.");
        }

        if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            throw new ValidationException("Only PNG, JPEG and WebP images are supported.");
        }

        await using var buffer = new MemoryStream();
        await file.Content.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();

        if (bytes.Length == 0)
        {
            throw new ValidationException("The uploaded certificate image is empty.");
        }

        if (!ImageDimensionReader.HasSupportedSignature(bytes))
        {
            throw new ValidationException("The uploaded file is not a valid PNG, JPEG or WebP image.");
        }

        (int Width, int Height) dimensions;
        try
        {
            dimensions = ImageDimensionReader.Read(bytes);
        }
        catch (InvalidDataException)
        {
            throw new ValidationException("The uploaded image could not be read. Use a valid PNG, JPEG or WebP file.");
        }

        if (dimensions.Width <= 0 || dimensions.Height <= 0 ||
            dimensions.Width > MaxDimension || dimensions.Height > MaxDimension)
        {
            throw new ValidationException("The uploaded image dimensions are not supported.");
        }

        await using var uploadStream = new MemoryStream(bytes, writable: false);

        var stored = await storage.UploadAsync(
            uploadStream,
            file.FileName,
            file.ContentType,
            $"certificate-templates/{eventId}",
            cancellationToken);

        return new CertificateTemplateImage(
            stored.Url,
            stored.PublicId,
            dimensions.Width,
            dimensions.Height);
    }

    public Task DeleteAsync(string publicId, CancellationToken cancellationToken = default) =>
        storage.DeleteAsync(publicId, cancellationToken);
}
