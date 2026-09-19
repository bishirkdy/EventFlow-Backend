using EventFlow.Event.Application.Abstractions.Storage;
using Microsoft.Extensions.Configuration;

namespace EventFlow.Event.Infrastructure.Storage
{
    public sealed class LocalFileStorage : IFileStorage
    {
        private static readonly IReadOnlyDictionary<string, string> ExtensionsByContentType =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["image/jpeg"] = ".jpg",
                ["image/png"] = ".png",
                ["image/webp"] = ".webp",
                ["image/gif"] = ".gif"
            };

        private const long MaxImageSizeBytes = 10 * 1024 * 1024;

        private readonly string _rootPath;
        private readonly string _requestPath;

        public LocalFileStorage(IConfiguration configuration)
        {
            var configuredRootPath = configuration["FileStorage:RootPath"];
            _rootPath = string.IsNullOrWhiteSpace(configuredRootPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                : Path.GetFullPath(configuredRootPath);

            _requestPath = configuration["FileStorage:RequestPath"] ?? "/uploads";

            if (!_requestPath.StartsWith('/'))
                _requestPath = "/" + _requestPath;

            _requestPath = _requestPath.TrimEnd('/');
        }

        public async Task<StoredFile> SaveAsync(
            UploadedFile file,
            string folder,
            CancellationToken cancellationToken = default)
        {
            if (!ExtensionsByContentType.TryGetValue(file.ContentType, out var extension))
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
            {
                file.Content.Position = 0;
            }

            var safeFolder = SanitizePath(folder);
            var storageFileName = $"{Guid.NewGuid():N}{extension}";
            var relativeStoragePath = Path.Combine(
                safeFolder.Replace('/', Path.DirectorySeparatorChar),
                storageFileName);

            var absolutePath = Path.Combine(_rootPath, relativeStoragePath);
            var directory = Path.GetDirectoryName(absolutePath);

            if (string.IsNullOrWhiteSpace(directory))
                throw new InvalidOperationException("Unable to resolve image storage directory.");

            Directory.CreateDirectory(directory);

            await using var target = new FileStream(
                absolutePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 64 * 1024,
                useAsync: true);

            if (!file.Content.CanSeek)
            {
                await target.WriteAsync(
                    header.AsMemory(0, headerBytesRead),
                    cancellationToken);
            }

            await file.Content.CopyToAsync(target, cancellationToken);

            var publicUrl = $"{_requestPath}/{relativeStoragePath.Replace('\\', '/') }";

            return new StoredFile(
                publicUrl,
                relativeStoragePath.Replace('\\', '/'),
                Path.GetFileName(file.FileName),
                file.ContentType,
                file.Length);
        }

        public Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var safeKey = SanitizePath(storageKey)
                .Replace('/', Path.DirectorySeparatorChar);

            var fullPath = Path.GetFullPath(Path.Combine(_rootPath, safeKey));
            var rootFullPath = Path.GetFullPath(_rootPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(rootFullPath, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Invalid image storage key.");

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }


        private static bool HasValidImageSignature(
            string contentType,
            ReadOnlySpan<byte> header,
            int headerLength)
        {
            return contentType.ToLowerInvariant() switch
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

        private static string SanitizePath(string path)
        {
            var normalized = path.Replace('\\', '/').Trim('/');

            if (string.IsNullOrWhiteSpace(normalized))
                throw new InvalidOperationException("Storage path is required.");

            var segments = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Any(segment => segment is "." or ".." || segment.Contains(Path.DirectorySeparatorChar) || segment.Contains(Path.AltDirectorySeparatorChar)))
                throw new InvalidOperationException("Invalid storage path.");

            return string.Join('/', segments);
        }
    }
}
