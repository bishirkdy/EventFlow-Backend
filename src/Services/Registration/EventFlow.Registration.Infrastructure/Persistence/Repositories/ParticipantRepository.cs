using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Infrastructure.Persistence.Repositories;

public sealed class ParticipantRepository(RegistrationDbContext db) : IParticipantRepository
{
    public async Task<Participant?> GetByIdAsync(Guid eventId,Guid participantId,bool asNoTracking = false,CancellationToken cancellationToken = default)
    {
        IQueryable<Participant> query = db.Participants;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(
            x => x.Id == participantId && x.EventId == eventId, cancellationToken);
    }

    public async Task<(IReadOnlyList<Participant> Items, int TotalCount)> GetPagedForEventAsync(
        Guid eventId,
        ParticipantStatus? status,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Participant> query = db.Participants
            .AsNoTracking()
            .Where(x => x.EventId == eventId);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();

            query = query.Where(
                x =>
                    x.ParticipantNumber.Contains(value) ||
                    x.FirstName.Contains(value) ||
                    x.LastName.Contains(value) ||
                    x.Email.Contains(value));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
