using EventFlow.Identity.Domain.Entities;

namespace EventFlow.Identity.Application.Abstractions.Repositories
{
    // Defines the operations the Application layer needs to persist and retrieve users without depending on the database implementation.
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid userId,CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(string email,CancellationToken cancellationToken);
        Task<User?> GetByUserNameAsync(string userName,CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task UpdateAsync(User user,CancellationToken cancellationToken);
    }
}
