using EventFlow.Registration.Application.Abstractions.Services;

namespace EventFlow.Registration.Api.Services;

public sealed class CertificateVerifyUrlProvider(IConfiguration configuration)
    : ICertificateVerifyUrlProvider
{
    public string GetUrl(string certificateNumber)
    {
        var baseUrl = configuration["Certificates:PublicBaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = "http://localhost:4200";
        }

        return $"{baseUrl.TrimEnd('/')}/verify/certificate/{certificateNumber}";
    }
}
