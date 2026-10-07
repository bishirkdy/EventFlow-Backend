
namespace EventFlow.Identity.Application.Abstractions.Authorization
{
    public interface IPermissionService
    {
        // Check user's role/permissions for the event
        Task<bool> HasPermissionAsync(Guid userId,Guid eventId,string permission,CancellationToken cancellationToken = default);
    }
}
