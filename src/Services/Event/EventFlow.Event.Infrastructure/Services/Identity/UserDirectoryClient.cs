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

        if (envelope is null || !envelope.IsSuccess)
            return null;

        return envelope.Data?.DisplayName;
    }

    private sealed record ApiResponse<T>(bool IsSuccess, int StatusCode, T? Data, string Message, List<string>? Errors);

    private sealed record UserSummary(
        Guid Id,
        string UserName,
        string FirstName,
        string LastName,
        string DisplayName);
}
