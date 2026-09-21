namespace EventFlow.Event.Application.Abstractions.Storage
{
    public sealed record StoredFile(
        string Url,
        string StorageKey,
        string FileName,
        string ContentType,
        long Length);
}
