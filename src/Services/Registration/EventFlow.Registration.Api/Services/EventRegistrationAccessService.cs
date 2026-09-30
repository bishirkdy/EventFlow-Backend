using System.Net.Http.Json;
using EventFlow.Registration.Application.Abstractions.Services;

namespace EventFlow.Registration.Api.Services;

public sealed class EventRegistrationAccessService(
    IHttpClientFactory factory, IConfiguration configuration) : IEventRegistrationAccessService
{
    public async Task<bool> IsRegistrationFeatureEnabledAsync(Guid eventId, CancellationToken ct = default)
    {
        var client = factory.CreateClient("EventService");
        AddInternalServiceKey(client, configuration);
        AddInternalServiceKey(client, configuration);
        using var response = await client.GetAsync(
            $"api/v1/events/{eventId}/features/registration",
            ct);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<FeatureResult>(cancellationToken: ct);
        return result?.Enabled == true;
    }

    public async Task<bool> CanManageRegistrationAsync(Guid eventId, Guid userId, CancellationToken ct = default)
    {
        var client = factory.CreateClient("EventService");
        using var response = await client.GetAsync(
            $"api/v1/events/{eventId}/registration-access?userId={userId}",
            ct);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<AccessResult>(cancellationToken: ct);
        return result?.Allowed == true;
    }

    private static void AddInternalServiceKey(HttpClient client, IConfiguration configuration)
    {
        var key = configuration["InternalService:Key"];
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("InternalService:Key is not configured.");
        }

        client.DefaultRequestHeaders.Remove("X-Internal-Service-Key");
        client.DefaultRequestHeaders.Add("X-Internal-Service-Key", key);
    }

    private sealed record FeatureResult(bool Enabled);
    private sealed record AccessResult(bool Allowed);
}
