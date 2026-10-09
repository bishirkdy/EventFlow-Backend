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

    public async Task<bool> AssignEventRoleAsync(
        Guid userId,
        Guid eventId,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        return await SendRoleCommandAsync(
            "assign-event-role",
            userId,
            eventId,
            roleName,
            cancellationToken);
    }

    public async Task<bool> RemoveEventRoleAsync(
        Guid userId,
        Guid eventId,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        return await SendRoleCommandAsync(
            "remove-event-role",
            userId,
            eventId,
            roleName,
            cancellationToken);
    }

    private async Task<bool> SendRoleCommandAsync(
        string action,
        Guid userId,
        Guid eventId,
        string roleName,
        CancellationToken cancellationToken)
    {
        var baseUrl = configuration["Services:Identity:BaseUrl"]
            ?? throw new InvalidOperationException(
                "Identity service URL is not configured.");

        var url =
            $"{baseUrl.TrimEnd('/')}/api/authorization/v1/{action}";

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            url)
        {
            Content = JsonContent.Create(new
            {
                UserId = userId,
                EventId = eventId,
                RoleName = roleName
            })
        };

        AddInternalServiceKey(request);

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(
                cancellationToken);

            throw new InvalidOperationException(
                $"Identity role removal failed. " +
                $"Status: {(int)response.StatusCode}, Response: {error}");
        }

        return true;
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