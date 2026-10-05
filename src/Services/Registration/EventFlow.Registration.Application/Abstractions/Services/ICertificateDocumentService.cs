namespace EventFlow.Registration.Application.Abstractions.Services;

public interface ICertificateDocumentService
{
    byte[] Generate(CertificateDocumentData data);
}
