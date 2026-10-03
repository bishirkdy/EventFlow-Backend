using EventFlow.Registration.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace EventFlow.Registration.Infrastructure.Services;

public sealed class FileCertificateStore(IConfiguration configuration) : ICertificateFileStore
{
    public async Task SaveAsync(
        Guid eventId,
        string fileName,
        byte[] content,
        CancellationToken cancellationToken = default)
    {
        var directory = GetDirectory(eventId);
        Directory.CreateDirectory(directory);

        await File.WriteAllBytesAsync(
            Path.Combine(directory, fileName),
            content,
            cancellationToken);
    }

    public async Task<Stream?> OpenReadAsync(
        Guid eventId,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(GetDirectory(eventId), fileName);

        if (!File.Exists(path))
        {
            return null;
        }

        return File.OpenRead(path);
    }

    private string GetDirectory(Guid eventId)
    {
        var root = configuration["Certificates:StoragePath"];

        if (string.IsNullOrWhiteSpace(root))
        {
            root = Path.Combine(
                AppContext.BaseDirectory,
                "certificate-storage");
        }

        return Path.Combine(root, eventId.ToString("N"));
    }
}
