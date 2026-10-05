namespace EventFlow.Operations.Application.Abstractions;

public interface IIdentityClient
{
    Task<IdentityUserSummary?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<IdentityUserSummary?> FindByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> AssignEventRoleAsync(
        Guid userId,
        Guid eventId,
        string roleName,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveEventRoleAsync(
        Guid userId,
        Guid eventId,
        string roleName,
        CancellationToken cancellationToken = default);
}
