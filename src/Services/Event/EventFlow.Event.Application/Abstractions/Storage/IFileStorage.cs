namespace EventFlow.Event.Application.Abstractions.Storage
{
    public sealed record UploadedFile(
        Stream Content,
        string FileName,
        string ContentType,
        long Length);

    public sealed record StoredFile(
        string Url,
        string StorageKey,
        string FileName,
        string ContentType,
        long Length);

    public interface IFileStorage
    {
        Task<StoredFile> SaveAsync(
            UploadedFile file,
            string folder,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken = default);
    }
}
