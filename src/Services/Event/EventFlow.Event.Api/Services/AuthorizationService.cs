using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Abstractions.Authorization;

namespace EventFlow.Event.Api.Services;

public sealed class AuthorizationService(HttpClient httpClient, IConfiguration configuration) : IAuthorizationService
{
    public async Task<bool> HasPermissionAsync(Guid userId, Guid eventId, string permission, CancellationToken cancellationToken = default)
    {
        var serviceKey = configuration["InternalService:Key"];

        if (string.IsNullOrWhiteSpace(serviceKey))
        {
            throw new InvalidOperationException("InternalService:Key is not configured.");
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "api/authorization/v1/check-permission")
        {
            Content = JsonContent.Create(new
            {
                UserId = userId,
                EventId = eventId,
                Permission = permission
            })
        };

        request.Headers.Add("X-Internal-Service-Key", serviceKey);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken);
        return result?.IsSuccess == true && result.Data;
    }
}
