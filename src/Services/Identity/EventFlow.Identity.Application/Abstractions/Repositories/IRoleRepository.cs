using EventFlow.Identity.Domain.Entities;


namespace EventFlow.Identity.Application.Abstractions.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
