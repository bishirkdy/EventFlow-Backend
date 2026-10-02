using Microsoft.Extensions.Configuration;

namespace EventFlow.Operations.Infrastructure.Services;

public sealed class EventAuthorizationClient(
    HttpClient httpClient,
    IConfiguration configuration)
{
    private readonly string _eventBaseUrl =
        configuration["Services:Event:BaseUrl"]
        ?? throw new InvalidOperationException(
            "Services:Event:BaseUrl is not configured.");

    public HttpClient Client => httpClient;

    public string BaseUrl => _eventBaseUrl;
}