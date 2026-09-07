using EventFlow.Identity.Domain.Entities;


namespace EventFlow.Identity.Application.Abstractions.Repositories
{
    //Interface for get permission
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
