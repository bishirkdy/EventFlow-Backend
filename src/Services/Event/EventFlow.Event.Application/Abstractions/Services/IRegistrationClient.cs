namespace EventFlow.Event.Application.Abstractions.Services;

public interface IRegistrationClient
{
    Task<IReadOnlyList<Guid>> GetEventIdsForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<int> GetParticipantCountAsync(
        CancellationToken cancellationToken = default);
}
