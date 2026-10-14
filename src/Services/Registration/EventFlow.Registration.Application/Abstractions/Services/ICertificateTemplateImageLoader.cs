namespace EventFlow.Registration.Application.Abstractions.Services;

public interface ICertificateTemplateImageLoader
{
    Task<byte[]?> LoadAsync(
        string url,
        CancellationToken cancellationToken = default);
}
