using System.Net.Http.Json;
using EventFlow.Contracts.Common;
using EventFlow.Operations.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace EventFlow.Operations.Infrastructure.Services;

public sealed class EventAuthorizationClient(
    HttpClient httpClient,
    IConfiguration configuration) : IEventAuthorizationClient
{
    public async Task<bool> HasPermissionAsync(
        Guid userId,
        Guid eventId,
        string permission,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = configuration["Services:Identity:BaseUrl"]
            ?? throw new InvalidOperationException("Services:Identity:BaseUrl is not configured.");

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{baseUrl.TrimEnd('/')}/api/authorization/v1/check-permission")
        {
            Content = JsonContent.Create(new
            {
                userId,
                eventId,
                permission,
            }),
        };

        var key = configuration["InternalService:Key"];
        if (!string.IsNullOrWhiteSpace(key))
            request.Headers.Add("X-Internal-Service-Key", key);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken);
        return result?.Data == true;
    }
}
