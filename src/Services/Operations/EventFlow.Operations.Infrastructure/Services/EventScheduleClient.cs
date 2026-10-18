using System.Net.Http.Json;
using EventFlow.Operations.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace EventFlow.Operations.Infrastructure.Services;

public sealed class EventScheduleClient(
    HttpClient httpClient,
    IConfiguration configuration) : IEventScheduleClient
{
    public async Task<int> GetSessionCountAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = ResolveBaseUrl();

        var result = await httpClient.GetFromJsonAsync<ApiListResponse>(
            $"{baseUrl}/api/v1/session/{eventId:D}/sessions",
            cancellationToken);

        return result?.Data?.Count ?? 0;
    }

    public async Task<Guid?> GetSessionSectionIdAsync(
        Guid eventId,
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = ResolveBaseUrl();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{baseUrl}/api/v1/events/{eventId:D}/sessions/{sessionId:D}/section-info");

        var key = configuration["InternalService:Key"];
        if (!string.IsNullOrWhiteSpace(key))
            request.Headers.Add("X-Internal-Service-Key", key);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<SectionInfoResponse>(cancellationToken);

        return result is { Exists: true } ? result.SectionId : null;
    }

    private string ResolveBaseUrl() =>
        (configuration["Services:Event:BaseUrl"]
            ?? throw new InvalidOperationException("Services:Event:BaseUrl is not configured.")).TrimEnd('/');

    private sealed record ApiListResponse(
        bool IsSuccess,
        int StatusCode,
        List<object>? Data,
        string? Message,
        List<string>? Errors);

    private sealed record SectionInfoResponse(bool Exists, Guid SectionId);
}
