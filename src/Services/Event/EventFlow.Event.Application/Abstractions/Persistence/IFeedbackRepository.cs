using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Domain.Enums;

namespace EventFlow.Event.Application.Abstractions.Persistence;

public interface IFeedbackRepository : IRepository<Feedback>
{
    Task<IReadOnlyList<Feedback>> GetByEventAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    Task<Feedback?> FindAsync(
        Guid eventId,
        Guid participantUserId,
        FeedbackTargetType targetType,
        Guid? targetId,
        CancellationToken cancellationToken = default);
}
