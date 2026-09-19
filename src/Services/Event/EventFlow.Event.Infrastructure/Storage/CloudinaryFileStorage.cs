using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Infrastructure.Storage.Cloudinary;

namespace EventFlow.Event.Infrastructure.Storage;

public sealed class CloudinaryFileStorage(ICloudinaryStorage cloudinaryStorage) : IFileStorage
{
    private static readonly IReadOnlyDictionary<string, string> SupportedContentTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/webp"] = ".webp",
            ["image/gif"] = ".gif",
        };

    private const long MaxImageSizeBytes = 10 * 1024 * 1024;

    public async Task<StoredFile> SaveAsync(
        UploadedFile file,
        string folder,
        CancellationToken cancellationToken = default)
    {
        if (!SupportedContentTypes.ContainsKey(file.ContentType))
            throw new InvalidOperationException("Unsupported image content type.");

        if (file.Length <= 0 || file.Length > MaxImageSizeBytes)
            throw new InvalidOperationException("Image size is invalid.");

        var header = new byte[12];
        var headerBytesRead = 0;

        while (headerBytesRead < header.Length)
        {
            var bytesRead = await file.Content.ReadAsync(
                header.AsMemory(headerBytesRead),
                cancellationToken);

            if (bytesRead == 0)
                break;

            headerBytesRead += bytesRead;
        }

        if (!HasValidImageSignature(file.ContentType, header, headerBytesRead))
            throw new InvalidOperationException("The uploaded file is not a valid image.");

        if (file.Content.CanSeek)
            file.Content.Position = 0;

        var stored = await cloudinaryStorage.UploadAsync(
            file.Content,
            file.FileName,
            file.ContentType,
            folder,
            cancellationToken);

        return new StoredFile(
            stored.Url,
            stored.PublicId,
            stored.FileName,
            stored.ContentType,
            file.Length);
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default) =>
        cloudinaryStorage.DeleteAsync(storageKey, cancellationToken);

    private static bool HasValidImageSignature(
        string contentType,
        ReadOnlySpan<byte> header,
        int headerLength) => contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => headerLength >= 3
                && header[0] == 0xFF
                && header[1] == 0xD8
                && header[2] == 0xFF,

            "image/png" => headerLength >= 8
                && header[..8].SequenceEqual(new byte[]
                {
                    0x89, 0x50, 0x4E, 0x47,
                    0x0D, 0x0A, 0x1A, 0x0A
                }),

            "image/gif" => headerLength >= 6
                && (header[..6].SequenceEqual("GIF87a"u8)
                    || header[..6].SequenceEqual("GIF89a"u8)),

            "image/webp" => headerLength >= 12
                && header[..4].SequenceEqual("RIFF"u8)
                && header[8..12].SequenceEqual("WEBP"u8),

            _ => false
        };
}
