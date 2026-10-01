using EventFlow.Identity.Domain.Entities;

namespace EventFlow.Identity.Application.Abstractions.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Role> GetOrCreateOwnerRoleAsync(CancellationToken cancellationToken = default);
}
