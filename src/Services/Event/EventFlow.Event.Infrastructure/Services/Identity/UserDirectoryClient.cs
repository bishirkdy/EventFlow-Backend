using System.Net.Http.Json;
using EventFlow.Event.Application.Abstractions.Services;

namespace EventFlow.Event.Infrastructure.Services.Identity;

public sealed class UserDirectoryClient(HttpClient httpClient) : IUserDirectoryClient
{
    public async Task<string?> GetDisplayNameAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"api/v1/users/{userId:D}/summary",
            cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<UserSummary>>(
            cancellationToken);

        return envelope?.Data?.DisplayName;
    }

    private sealed record ApiResponse<T>(bool Success, string Message, T? Data);

    private sealed record UserSummary(
        Guid Id,
        string UserName,
        string FirstName,
        string LastName,
        string DisplayName);
}
