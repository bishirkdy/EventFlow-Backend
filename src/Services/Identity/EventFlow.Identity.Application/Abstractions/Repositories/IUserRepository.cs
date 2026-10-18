using EventFlow.Contracts.Common;
using EventFlow.Identity.Domain.Entities;

namespace EventFlow.Identity.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken);

    Task AddAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken);

    // Get users by pagination for the admin console.
    Task<PaginatedResponse<User>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken);

    // Get aggregate user counts in a single query.
    Task<Abstractions.ReadModels.UserCountStats> GetCountsAsync(CancellationToken cancellationToken);

}
