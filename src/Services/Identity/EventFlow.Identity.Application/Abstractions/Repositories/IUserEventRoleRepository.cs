using EventFlow.Identity.Domain.Entities;


namespace EventFlow.Identity.Application.Abstractions.Repositories
{
    public interface IUserEventRoleRepository
    {
        Task<bool> ExistsAsync(
            Guid userId,
            Guid eventId,
            Guid roleId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            UserEventRole userEventRole,
            CancellationToken cancellationToken = default);

        Task<List<UserEventRole>> GetByUserAndEventAsync(
            Guid userId,
            Guid eventId,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            UserEventRole userEventRole);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
