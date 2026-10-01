using System.Net.Http.Json;
using EventFlow.Event.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace EventFlow.Event.Infrastructure.Services.Identity;

// Client used to communicate with the Identity Service.
public sealed class UserDirectoryClient(
    HttpClient httpClient,
    IConfiguration configuration) : IUserDirectoryClient
{
    private readonly string _serviceKey =
        configuration["InternalService:Key"]
        ?? throw new InvalidOperationException(
            "InternalService:Key is not configured.");

    // Gets the display name of a user by user ID.
    public async Task<string?> GetDisplayNameAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"api/v1/users/{userId:D}/summary",
            cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var envelope =
            await response.Content.ReadFromJsonAsync<ApiResponse<UserSummary>>(
                cancellationToken);

        if (envelope is null || !envelope.IsSuccess)
        {
            return null;
        }

        return envelope.Data?.DisplayName;
    }

    // Assigns the event creator as the Owner of the event.
    public async Task AssignOwnerAsync(
        Guid userId,
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "api/authorization/v1/assign-owner")
        {
            Content = JsonContent.Create(new
            {
                UserId = userId,
                EventId = eventId
            })
        };

        request.Headers.Add(
            "X-Internal-Service-Key",
            _serviceKey);

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    // Represents the standard API response from the Identity Service.
    private sealed record ApiResponse<T>(
        bool IsSuccess,
        int StatusCode,
        T? Data,
        string Message,
        List<string>? Errors);

    // Represents the user information returned by the Identity Service.
    private sealed record UserSummary(
        Guid Id,
        string UserName,
        string FirstName,
        string LastName,
        string DisplayName);
}