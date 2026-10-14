namespace EventFlow.Registration.Application.Abstractions.Services;

public sealed record UploadedFile(
    Stream Content,
    string FileName,
    string ContentType,
    long Length);
