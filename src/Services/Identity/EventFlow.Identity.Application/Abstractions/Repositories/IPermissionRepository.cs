using EventFlow.Identity.Domain.Entities;


namespace EventFlow.Identity.Application.Abstractions.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
