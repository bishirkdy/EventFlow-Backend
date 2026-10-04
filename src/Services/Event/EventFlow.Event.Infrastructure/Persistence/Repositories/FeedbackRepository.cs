using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Persistence.Repositories
{
    public sealed class FeedbackRepository(EventCoreDbContext context)
        : GenericRepository<Feedback>(context), IFeedbackRepository
    {
        public async Task<IReadOnlyList<Feedback>> GetByEventAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(x => x.EventId == eventId)
                .OrderByDescending(x => x.SubmittedAtUtc)
                .ToListAsync(cancellationToken);
        }

        public async Task<Feedback?> FindAsync(
            Guid eventId,
            Guid participantUserId,
            FeedbackTargetType targetType,
            Guid? targetId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(
                    x => x.EventId == eventId
                         && x.ParticipantUserId == participantUserId
                         && x.TargetType == targetType
                         && x.TargetId == targetId,
                    cancellationToken);
        }
    }
}
