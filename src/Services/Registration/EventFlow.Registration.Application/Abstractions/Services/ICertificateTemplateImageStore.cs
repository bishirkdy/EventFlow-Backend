namespace EventFlow.Registration.Application.Abstractions.Services;

public interface ICertificateTemplateImageStore
{
    Task<CertificateTemplateImage> SaveAsync(
        Guid eventId,
        UploadedFile file,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string publicId,
        CancellationToken cancellationToken = default);
}
