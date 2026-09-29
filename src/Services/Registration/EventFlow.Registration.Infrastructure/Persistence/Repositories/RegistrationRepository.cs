using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;
using Microsoft.EntityFrameworkCore;

using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Infrastructure.Persistence.Repositories;

public sealed class RegistrationRepository(RegistrationDbContext db)
    : IRegistrationRepository
{
    public async Task<RegistrationEntity?> GetByIdAsync(
        Guid eventId,
        Guid registrationId,
        Guid? userId = null,
        bool includeParticipant = false,
        bool includeTicket = false,
        bool includeAnswers = false,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<RegistrationEntity> query = db.Registrations;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (includeParticipant)
        {
            query = query.Include(x => x.Participant);
        }

        if (includeTicket)
        {
            query = query.Include(x => x.Ticket);
        }

        if (includeAnswers)
        {
            query = query.Include(x => x.Answers);
        }

        query = query.Where(
            x => x.Id == registrationId && x.EventId == eventId);

        if (userId.HasValue)
        {
            query = query.Where(x => x.UserId == userId.Value);
        }

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RegistrationEntity>> GetForUserAsync(
        Guid userId,
        Guid? eventId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<RegistrationEntity> query = db.Registrations
            .AsNoTracking()
            .Include(x => x.Participant)
            .Include(x => x.Ticket)
            .Where(x => x.UserId == userId);

        if (eventId.HasValue)
        {
            query = query.Where(x => x.EventId == eventId.Value);
        }

        return await query
            .OrderByDescending(x => x.RegisteredAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<RegistrationEntity> Items, int TotalCount)> GetPagedForEventAsync(
        Guid eventId,
        RegistrationStatus? status,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<RegistrationEntity> query = db.Registrations
            .AsNoTracking()
            .Include(x => x.Participant)
            .Include(x => x.Ticket)
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
                    x.RegistrationNumber.Contains(value) ||
                    (x.Participant != null &&
                     (x.Participant.FirstName.Contains(value) ||
                      x.Participant.LastName.Contains(value) ||
                      x.Participant.Email.Contains(value) ||
                      x.Participant.ParticipantNumber.Contains(value))));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.RegisteredAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<bool> HasActiveRegistrationAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return db.Registrations.AnyAsync(
            x =>
                x.EventId == eventId &&
                x.UserId == userId &&
                x.Status != RegistrationStatus.Cancelled,
            cancellationToken);
    }

    public Task<int> CountByStatusAsync(
        Guid eventId,
        RegistrationStatus status,
        CancellationToken cancellationToken = default)
    {
        return db.Registrations.CountAsync(
            x => x.EventId == eventId && x.Status == status,
            cancellationToken);
    }

    public async Task<RegistrationStatisticsReadModel> GetStatisticsAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var statistics = await db.Registrations
            .Where(x => x.EventId == eventId)
            .GroupBy(_ => 1)
            .Select(
                group => new RegistrationStatisticsReadModel(
                    Total: group.Count(),
                    Pending: group.Count(
                        x => x.Status == RegistrationStatus.Pending),
                    Approved: group.Count(
                        x => x.Status == RegistrationStatus.Approved),
                    Rejected: group.Count(
                        x => x.Status == RegistrationStatus.Rejected),
                    Cancelled: group.Count(
                        x => x.Status == RegistrationStatus.Cancelled),
                    Waitlisted: group.Count(
                        x => x.Status == RegistrationStatus.Waitlisted),
                    Participants: group.Count(
                        x => x.Participant != null),
                    ActiveTickets: group.Count(
                        x => x.Ticket != null && x.Ticket.IsActive)))
            .SingleOrDefaultAsync(cancellationToken);

        return statistics
            ?? new RegistrationStatisticsReadModel(
                Total: 0,
                Pending: 0,
                Approved: 0,
                Rejected: 0,
                Cancelled: 0,
                Waitlisted: 0,
                Participants: 0,
                ActiveTickets: 0);
    }

    public void Add(RegistrationEntity registration)
    {
        db.Registrations.Add(registration);
    }

    public void RemoveAnswers(IEnumerable<RegistrationAnswer> answers)
    {
        db.RegistrationAnswers.RemoveRange(answers);
    }
}
