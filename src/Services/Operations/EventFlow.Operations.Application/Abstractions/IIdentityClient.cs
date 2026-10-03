namespace EventFlow.Operations.Application.Abstractions;

public sealed record IdentityUserSummary(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string DisplayName);

public interface IIdentityClient
{
    Task<IdentityUserSummary?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<IdentityUserSummary?> FindByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}