namespace EventFlow.Infrastructure.Storage.Cloudinary;

public sealed record CloudinaryStoredFile(
    string Url,
    string PublicId,
    string FileName,
    string ContentType,
    long Length);

public interface ICloudinaryStorage
{
    Task<CloudinaryStoredFile> UploadAsync(Stream content,string fileName,string contentType, string folder,CancellationToken cancellationToken = default);

    Task DeleteAsync(string publicId, CancellationToken cancellationToken = default);
}
