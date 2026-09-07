using EventFlow.Identity.Domain.Entities;


namespace EventFlow.Identity.Application.Abstractions.Repositories
{
    //Interface for role handling
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
