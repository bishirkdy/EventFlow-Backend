

namespace EventFlow.Event.Application.Abstractions.Storage
{
    public sealed record UploadedFile(
        Stream Content,
        string FileName,
        string ContentType,
        long Length);

}
