namespace EventFlow.Operations.Application.Abstractions;

public interface IEventAuthorizationClient
{
    Task<bool> HasPermissionAsync(Guid userId, Guid eventId, string permission, CancellationToken cancellationToken = default);
}
