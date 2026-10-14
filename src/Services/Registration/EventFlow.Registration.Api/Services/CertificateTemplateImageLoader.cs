using EventFlow.Registration.Application.Abstractions.Services;

namespace EventFlow.Registration.Api.Services;

public sealed class CertificateTemplateImageLoader(
    IHttpClientFactory httpClientFactory,
    ILogger<CertificateTemplateImageLoader> logger)
    : ICertificateTemplateImageLoader
{
    private const string ClientName = "CertificateTemplateImages";
    private const long MaxBytes = 15 * 1024 * 1024;

    public async Task<byte[]?> LoadAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        try
        {
            var client = httpClientFactory.CreateClient(ClientName);

            using var response = await client.GetAsync(
                url,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Certificate template image request to {Url} failed with status {StatusCode}.",
                    url,
                    response.StatusCode);
                return null;
            }

            if (response.Content.Headers.ContentLength is > MaxBytes)
            {
                logger.LogWarning(
                    "Certificate template image at {Url} exceeds the maximum allowed size.",
                    url);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer, cancellationToken);

            if (buffer.Length == 0 || buffer.Length > MaxBytes)
            {
                logger.LogWarning(
                    "Certificate template image at {Url} was empty or exceeded the maximum allowed size.",
                    url);
                return null;
            }

            return buffer.ToArray();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Failed to download certificate template image from {Url}.",
                url);
            return null;
        }
    }
}
