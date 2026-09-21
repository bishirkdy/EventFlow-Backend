namespace EventFlow.Event.Application.Abstractions.Storage
{
    public interface IFileStorage
    {
        Task<StoredFile> SaveAsync(UploadedFile file,string folder, CancellationToken cancellationToken = default);
        Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
    }
}
