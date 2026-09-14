

namespace EventFlow.Event.Application.Abstractions.Authorization
{
    public interface IAuthorizationService
    {
        Task<bool> HasPermissionAsync(
            Guid userId,
            Guid eventId,
            string permission,
            CancellationToken cancellationToken = default);
    }
}
