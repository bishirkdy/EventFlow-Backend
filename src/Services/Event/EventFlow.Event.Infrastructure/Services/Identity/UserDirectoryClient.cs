using System.Net.Http.Json;
using EventFlow.Event.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
namespace EventFlow.Event.Infrastructure.Services.Identity;

public sealed class UserDirectoryClient(HttpClient httpClient, IConfiguration configuration) : IUserDirectoryClient
{
    private readonly string _serviceKey =configuration["InternalService:Key"]
        ?? throw new InvalidOperationException("InternalService Key is not configured.");

    public async Task<string?> GetDisplayNameAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/users/{userId:D}/summary");

        request.Headers.Add("X-Internal-Service-Key", _serviceKey);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<UserSummary>>(cancellationToken);

        if (envelope is null || !envelope.IsSuccess)
        {
            return null;
        }

        return envelope.Data?.DisplayName;
    }

    public async Task AssignOwnerAsync(Guid userId,Guid eventId,CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/authorization/v1/assign-owner")
        {
            Content = JsonContent.Create(new
            {
                UserId = userId,
                EventId = eventId
            })
        };

        request.Headers.Add("X-Internal-Service-Key", _serviceKey);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<Guid>> GetEventIdsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/users/{userId:D}/event-roles");

        request.Headers.Add("X-Internal-Service-Key", _serviceKey);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<UserEventRoleLookup>>>(cancellationToken);

        return envelope?.Data?
            .Select(x => x.EventId)
            .Distinct()
            .ToArray()
            ?? [];
    }

    private sealed record ApiResponse<T>(bool IsSuccess, int StatusCode, T? Data, string Message, List<string>? Errors);

    private sealed record UserEventRoleLookup(Guid EventId, IReadOnlyList<string> RoleNames);

    private sealed record UserSummary(
        Guid Id,
        string UserName,
        string FirstName,
        string LastName,
        string DisplayName);
}
