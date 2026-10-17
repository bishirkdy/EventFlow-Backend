using System.Net.Http.Json;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Contracts.Events;

namespace EventFlow.Registration.Api.Services;

//Asks the Event Service whether registration is enabled and whether a user may manage registration for an event.
public sealed class EventRegistrationAccessService(IHttpClientFactory factory, IConfiguration cfg) : IEventRegistrationAccessService
{
    public async Task<bool> IsRegistrationFeatureEnabledAsync(Guid eventId, CancellationToken ct = default)
    {
        if (cfg.GetValue<bool>("Registration:EventAccess:AllowAll")) return true;

        var client = factory.CreateClient("EventService");
        using var request = CreateInternalRequest(HttpMethod.Get, $"api/v1/events/{eventId}/features/registration");
        using var response = await client.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ct);
        return result?.Enabled == true;
    }

    public async Task<bool> IsEventPublishedAsync(Guid eventId, CancellationToken ct = default)
    {
        // Publish state is always verified against the Event service; the
        // dev-only AllowAll switch must not make unpublished events registerable.
        var client = factory.CreateClient("EventService");
        using var request = CreateInternalRequest(HttpMethod.Get, $"api/v1/events/{eventId}/public-status");
        using var response = await client.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ct);
        return result?.Published == true;
    }

    public async Task<bool> CanPreviewEventAsync(Guid eventId, Guid userId, CancellationToken ct = default)
    {
        var client = factory.CreateClient("EventService");
        using var request = CreateInternalRequest(HttpMethod.Get, $"api/v1/events/{eventId}/preview-access?userId={userId}");
        using var response = await client.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ct);
        return result?.Allowed == true;
    }

    public async Task<bool> CanManageRegistrationAsync(Guid eventId, Guid userId, CancellationToken ct = default)
    {
        if (cfg.GetValue<bool>("Registration:EventAccess:AllowAll")) return true;

        var client = factory.CreateClient("EventService");
        using var request = CreateInternalRequest(HttpMethod.Get, $"api/v1/events/{eventId}/registration-access?userId={userId}");
        using var response = await client.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: ct);
        return result?.Allowed == true;
    }

    public async Task<SessionRegistrationInfo?> GetSessionRegistrationInfoAsync(Guid eventId, Guid sessionId, CancellationToken ct = default)
    {
        var client = factory.CreateClient("EventService");
        using var request = CreateInternalRequest(HttpMethod.Get, $"api/v1/events/{eventId}/sessions/{sessionId}/registration-info");
        using var response = await client.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<SessionInfo>(cancellationToken: ct);

        if (result is null) return null;

        return new SessionRegistrationInfo(
            result.Exists,
            result.EventId,
            result.SessionId,
            result.SectionId,
            result.AllowRegistration,
            result.Capacity,
            result.Status ?? string.Empty);
    }

    // The Event Service exposes these endpoints to internal callers only, so the shared key must be attached.
    private HttpRequestMessage CreateInternalRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);

        var key = cfg["InternalService:Key"];
        if (!string.IsNullOrWhiteSpace(key))
            request.Headers.Add("X-Internal-Service-Key", key);

        return request;
    }

    sealed class Result
    {
        public bool Enabled { get; set; }
        public bool Allowed { get; set; }
        public bool Published { get; set; }
    }

    sealed record SessionInfo(
        bool Exists,
        Guid EventId,
        Guid SessionId,
        Guid SectionId,
        bool AllowRegistration,
        int? Capacity,
        string? Status);
}
