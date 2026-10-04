using System.Net.Http.Json;
using EventFlow.Event.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace EventFlow.Event.Infrastructure.Services.Registration;

public sealed class RegistrationClient(
    HttpClient httpClient,
    IConfiguration configuration) : IRegistrationClient
{
    private readonly string _serviceKey =
        configuration["InternalService:Key"]
        ?? throw new InvalidOperationException(
            "InternalService:Key is not configured.");

    public async Task<IReadOnlyList<Guid>> GetEventIdsForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/v1/registrations/internal/users/{userId:D}/event-ids");

        request.Headers.Add("X-Internal-Service-Key", _serviceKey);

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var envelope = await response.Content.ReadFromJsonAsync<
            ApiResponse<IReadOnlyList<Guid>>>(cancellationToken);

        return envelope?.Data ?? [];
    }

    private sealed record ApiResponse<T>(
        bool IsSuccess,
        int StatusCode,
        T? Data,
        string? Message,
        List<string>? Errors);
}
