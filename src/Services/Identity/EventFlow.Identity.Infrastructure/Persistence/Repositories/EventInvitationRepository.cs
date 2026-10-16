using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Persistence.Repositories
{
    // Repository responsible for database operations related to event invitations.
    public sealed class EventInvitationRepository(IdentityDbContext context): IEventInvitationRepository
    {
        // Get an event invitation by its id.
        public async Task<EventInvitation?> GetByIdAsync(Guid invitationId, CancellationToken cancellationToken)
        {
            return await context.EventInvitations
                .Include(x => x.Role)
                .Include(x => x.CreatedByUser)
                .Include(x => x.AcceptedByUser)
                .FirstOrDefaultAsync(x => x.Id == invitationId, cancellationToken);
        }

        // Get an event invitation using its invitation token.
        public async Task<EventInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            // Query the invitation and load its related Role, CreatedByUser, and AcceptedByUser.
            return await context.EventInvitations
                .Include(x => x.Role)
                .Include(x => x.CreatedByUser)
                .Include(x => x.AcceptedByUser)
                .FirstOrDefaultAsync(
                    x => x.Token == token, cancellationToken);
        }

        // Get an invitation for a specific event, type and email address.
        public async Task<EventInvitation?> GetByEventAndEmailAsync(Guid eventId,string email,EventInvitationType type,CancellationToken cancellationToken)
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

        // Get all event invitations belonging to a specific event, optionally filtered by type.
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

        // Get only pending event invitations for a specific event.
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

        // Add a new event invitation to the database.
        public async Task AddAsync(EventInvitation invitation, CancellationToken cancellationToken)
        {
            await context.EventInvitations.AddAsync(invitation, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        // Update an existing event invitation in the database
        public async Task UpdateAsync(EventInvitation invitation, CancellationToken cancellationToken)
        {
            // Mark the invitation as modified in the EF Core change tracker
            context.EventInvitations.Update(invitation);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
