namespace EventFlow.Registration.Application.Abstractions.Services;

public interface ICertificateTemplateBackgroundResolver
{
    Task<byte[]> ResolveAsync(
        CertificateSettings settings,
        CancellationToken cancellationToken = default);
}
