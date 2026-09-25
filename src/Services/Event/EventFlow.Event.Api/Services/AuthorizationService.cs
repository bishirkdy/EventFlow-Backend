using EventFlow.Event.Application.Abstractions.Authorization;

namespace EventFlow.Event.Api.Services
{
    public sealed class AuthorizationService(HttpClient _httpClient) : IAuthorizationService
    {

        public async Task<bool> HasPermissionAsync(Guid userId,Guid eventId,string permission,CancellationToken cancellationToken = default)
        {
            var request = new
            {
                UserId = userId,
                EventId = eventId,
                Permission = permission
            };

            var response = await _httpClient.PostAsJsonAsync("api/authorization/v1/check-permission",request,cancellationToken);

            response.EnsureSuccessStatusCode();

            var result =
                await response.Content.ReadFromJsonAsync<PermissionCheckResponse>(
                    cancellationToken);

            return result?.HasPermission ?? false;
        }

        private sealed record PermissionCheckResponse(bool HasPermission);
    }
}
