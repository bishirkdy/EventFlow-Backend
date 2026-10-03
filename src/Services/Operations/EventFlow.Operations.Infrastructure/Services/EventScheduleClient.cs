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
        var baseUrl = configuration["Services:Event:BaseUrl"]
            ?? throw new InvalidOperationException("Services:Event:BaseUrl is not configured.");

        var result = await httpClient.GetFromJsonAsync<ApiListResponse>(
            $"{baseUrl.TrimEnd('/')}/api/v1/session/{eventId:D}/sessions",
            cancellationToken);

        return result?.Data?.Count ?? 0;
    }

    private sealed record ApiListResponse(
        bool IsSuccess,
        int StatusCode,
        List<object>? Data,
        string? Message,
        List<string>? Errors);
}
