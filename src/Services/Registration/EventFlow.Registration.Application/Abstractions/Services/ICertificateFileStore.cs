namespace EventFlow.Registration.Application.Abstractions.Services;

public interface ICertificateFileStore
{
    Task SaveAsync(
        Guid eventId,
        string fileName,
        byte[] content,
        CancellationToken cancellationToken = default);

    Task<Stream?> OpenReadAsync(
        Guid eventId,
        string fileName,
        CancellationToken cancellationToken = default);
}
