using System.Net.Http.Json;
using EventFlow.Event.Application.Abstractions.Services;

namespace EventFlow.Event.Infrastructure.Services.Identity;

// Client used to communicate with the Identity Service
public sealed class UserDirectoryClient(HttpClient httpClient) : IUserDirectoryClient
{
    // Gets the display name of a user by user ID.
    public async Task<string?> GetDisplayNameAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Send a GET request to the Identity Service.
        using var response = await httpClient.GetAsync(
            $"api/v1/users/{userId:D}/summary", cancellationToken);

        // If the user does not exist, return null.
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        // Throw an exception if the response is not successful.
        response.EnsureSuccessStatusCode();

        // Convert the JSON response into ApiResponse<UserSummary>.
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<UserSummary>>(cancellationToken);

        // If the response is empty or indicates failure, return null.
        if (envelope is null || !envelope.IsSuccess)
        {
            return null;
        }

        // Return the user's display name.
        return envelope.Data?.DisplayName;
    }

    public async Task AssignOwnerAsync(
    Guid userId,
    Guid eventId,
    CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/authorization/v1/assign-owner",
            new
            {
                UserId = userId,
                EventId = eventId
            },
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
    // Represents the standard API response from the Identity Service.
    private sealed record ApiResponse<T>(bool IsSuccess,int StatusCode,T? Data,string Message,List<string>? Errors);

    // Represents the user information returned by the Identity Service.
    private sealed record UserSummary(
        Guid Id,
        string UserName,
        string FirstName,
        string LastName,
        string DisplayName);
}