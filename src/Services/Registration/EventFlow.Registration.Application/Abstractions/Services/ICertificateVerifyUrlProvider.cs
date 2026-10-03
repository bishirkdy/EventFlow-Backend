namespace EventFlow.Registration.Application.Abstractions.Services;

public interface ICertificateVerifyUrlProvider
{
    string GetUrl(string certificateNumber);
}
