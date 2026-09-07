
namespace EventFlow.Identity.Application.Abstractions.Authorization
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(
            Guid userId,
            Guid eventId,
            string permission,
            CancellationToken cancellationToken = default);
    }
}
