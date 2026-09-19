using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace EventFlow.Infrastructure.Storage.Cloudinary;

public sealed class CloudinaryStorage : ICloudinaryStorage
{
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;

    public CloudinaryStorage(IOptions<CloudinaryStorageOptions> options)
    {
        var settings = options.Value;

        if (string.IsNullOrWhiteSpace(settings.CloudName) ||
            string.IsNullOrWhiteSpace(settings.ApiKey) ||
            string.IsNullOrWhiteSpace(settings.ApiSecret))
        {
            throw new InvalidOperationException(
                "Cloudinary configuration is missing. Configure Cloudinary:CloudName, Cloudinary:ApiKey and Cloudinary:ApiSecret.");
        }

        var account = new Account(
            settings.CloudName,
            settings.ApiKey,
            settings.ApiSecret);

        _cloudinary = new CloudinaryDotNet.Cloudinary(account);
    }

    public async Task<CloudinaryStoredFile> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (!content.CanRead)
            throw new InvalidOperationException("The image stream cannot be read.");

        if (content.CanSeek)
            content.Position = 0;

        cancellationToken.ThrowIfCancellationRequested();

        var safeFileName = Path.GetFileName(fileName);
        var publicId = $"{Guid.NewGuid():N}";

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(safeFileName, content),
            Folder = folder,
            PublicId = publicId,
            UseFilename = false,
            UniqueFilename = false,
            Overwrite = false,
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        cancellationToken.ThrowIfCancellationRequested();

        if (result.Error is not null)
        {
            throw new InvalidOperationException(
                $"Cloudinary upload failed: {result.Error.Message}");
        }

        if (result.SecureUrl is null || string.IsNullOrWhiteSpace(result.PublicId))
            throw new InvalidOperationException("Cloudinary did not return a valid uploaded image.");

        var length = content.CanSeek ? content.Length : 0;

        return new CloudinaryStoredFile(
            result.SecureUrl.ToString(),
            result.PublicId,
            safeFileName,
            contentType,
            length);
    }

    public async Task DeleteAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return;

        cancellationToken.ThrowIfCancellationRequested();

        var result = await _cloudinary.DestroyAsync(
            new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image,
                Type = "upload",
            });

        if (result.Error is not null &&
            !string.Equals(result.Result, "not found", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Cloudinary delete failed: {result.Error.Message}");
        }
    }
}
