namespace EventFlow.Registration.Application.Abstractions.Services;

public sealed record CertificateTemplateImage(
    string Url,
    string PublicId,
    int Width,
    int Height);
