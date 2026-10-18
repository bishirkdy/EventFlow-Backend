using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Persistence.Repositories;

public sealed class EventInvitationRepository(IdentityDbContext context) : IEventInvitationRepository
{
    public async Task<EventInvitation?> GetByIdAsync(Guid invitationId, CancellationToken cancellationToken)
    {
        return await context.EventInvitations
            .Include(x => x.Role)
            .Include(x => x.CreatedByUser)
            .Include(x => x.AcceptedByUser)
            .FirstOrDefaultAsync(x => x.Id == invitationId, cancellationToken);
    }

    public async Task<EventInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await context.EventInvitations
            .Include(x => x.Role)
            .Include(x => x.CreatedByUser)
            .Include(x => x.AcceptedByUser)
            .FirstOrDefaultAsync(
                x => x.Token == token, cancellationToken);
    }

    public async Task<EventInvitation?> GetByEventAndEmailAsync(Guid eventId, string email, EventInvitationType type, CancellationToken cancellationToken)
    {
        return await context.EventInvitations
            .Include(x => x.Role)
            .Include(x => x.CreatedByUser)
            .Include(x => x.AcceptedByUser)
            .FirstOrDefaultAsync(
                x => x.EventId == eventId
                     && x.Type == type
                     && x.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<IReadOnlyList<EventInvitation>> GetByEventIdAsync(Guid eventId, EventInvitationType? type, CancellationToken cancellationToken)
    {
        var query = context.EventInvitations
            .Include(x => x.Role)
            .Include(x => x.CreatedByUser)
            .Include(x => x.AcceptedByUser)
            .Where(x => x.EventId == eventId);

        if (type is not null)
        {
            query = query.Where(x => x.Type == type);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventInvitation>> GetPendingByEventIdAsync(Guid eventId,
        CancellationToken cancellationToken)
    {
        return await context.EventInvitations
            .Include(x => x.Role)
            .Include(x => x.CreatedByUser)
            .Include(x => x.AcceptedByUser)
            .Where(x =>
                x.EventId == eventId &&
                x.Status == InvitationStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(EventInvitation invitation, CancellationToken cancellationToken)
    {
        await context.EventInvitations.AddAsync(invitation, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(EventInvitation invitation, CancellationToken cancellationToken)
    {
        context.EventInvitations.Update(invitation);
        await context.SaveChangesAsync(cancellationToken);
    }
}
