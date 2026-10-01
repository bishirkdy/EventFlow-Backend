namespace EventFlow.Event.Application.Abstractions.Services;

public interface IUserDirectoryClient
{
    Task<string?> GetDisplayNameAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AssignOwnerAsync(
    Guid userId,
    Guid eventId,
    CancellationToken cancellationToken = default);
}
