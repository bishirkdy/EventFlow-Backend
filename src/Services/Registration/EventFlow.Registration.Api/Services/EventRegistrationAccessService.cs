using System.Net.Http.Json;
using EventFlow.Registration.Application.Abstractions.Services;
namespace EventFlow.Registration.Api.Services;

public sealed class EventRegistrationAccessService(
    IHttpClientFactory factory,
    IConfiguration cfg
) : IEventRegistrationAccessService
{
    public async Task<bool> IsRegistrationFeatureEnabledAsync(Guid eventId, CancellationToken ct = default)
    {
        if (cfg.GetValue<bool>("Registration:EventAccess:AllowAll")) return true;
        var c = factory.CreateClient("EventService");
        var r = await c.GetAsync($"api/v1/events/{eventId}/features/registration", ct);
        if (!r.IsSuccessStatusCode) return false;
        var x = await r.Content.ReadFromJsonAsync<Result>(cancellationToken: ct);
        return x?.Enabled == true;
    }
    public async Task<bool> CanManageRegistrationAsync(Guid eventId, Guid userId, CancellationToken ct = default)
    {
        if (cfg.GetValue<bool>("Registration:EventAccess:AllowAll")) return true;
        var c = factory.CreateClient("EventService");
        var r = await c.GetAsync($"api/v1/events/{eventId}/registration-access", ct);
        if (!r.IsSuccessStatusCode) return false;
        var x = await r.Content.ReadFromJsonAsync<Result>(cancellationToken: ct);
        return x?.Allowed == true;
    }
    sealed class Result
    {
        public bool Enabled {get;set;}
        public bool Allowed {get;set;}
    }
}
