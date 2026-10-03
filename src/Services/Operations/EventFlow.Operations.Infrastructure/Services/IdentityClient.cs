using System.Net.Http.Json;
using EventFlow.Contracts.Common;
using EventFlow.Operations.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace EventFlow.Operations.Infrastructure.Services;

public sealed class IdentityClient(
    HttpClient httpClient,
    IConfiguration configuration) : IIdentityClient
{
    public async Task<IdentityUserSummary?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = configuration["Services:Identity:BaseUrl"]
            ?? throw new InvalidOperationException(
                "Identity service URL is not configured.");

        var url =
            $"{baseUrl.TrimEnd('/')}/api/v1/users/by-email?email={Uri.EscapeDataString(email)}";

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            url);

        AddInternalServiceKey(request);

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var result =
            await response.Content.ReadFromJsonAsync<
                ApiResponse<IdentityUserSummary>>(
                cancellationToken);

        return result?.Data;
    }

    public async Task<IdentityUserSummary?> FindByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = configuration["Services:Identity:BaseUrl"]
            ?? throw new InvalidOperationException(
                "Identity service URL is not configured.");

        var url =
            $"{baseUrl.TrimEnd('/')}/api/v1/users/{userId}/summary";

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            url);

        AddInternalServiceKey(request);

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var result =
            await response.Content.ReadFromJsonAsync<
                ApiResponse<IdentityUserSummary>>(
                cancellationToken);

        return result?.Data;
    }

    private void AddInternalServiceKey(HttpRequestMessage request)
    {
        var key = configuration["InternalService:Key"];

        if (!string.IsNullOrWhiteSpace(key))
        {
            request.Headers.Add(
                "X-Internal-Service-Key",
                key);
        }
    }
}