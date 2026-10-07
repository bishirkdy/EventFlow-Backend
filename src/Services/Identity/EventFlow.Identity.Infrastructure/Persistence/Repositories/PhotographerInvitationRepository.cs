using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Persistence.Repositories
{
    // Repository responsible for database operations related to photographer invitations.
    public sealed class PhotographerInvitationRepository(IdentityDbContext context): IPhotographerInvitationRepository
    {
        // Get a photographer invitation
        public async Task<PhotographerInvitation?> GetByIdAsync(Guid invitationId, CancellationToken cancellationToken)
        {
            return await context.PhotographerInvitations
                .Include(x => x.Role)
                .Include(x => x.CreatedByUser)
                .Include(x => x.AcceptedByUser)
                .FirstOrDefaultAsync(x => x.Id == invitationId, cancellationToken);
        }

        // Get a photographer invitation using its invitation token.
        public async Task<PhotographerInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            // Query the invitation and load its related Role, CreatedByUser, and AcceptedByUser.
            return await context.PhotographerInvitations
                .Include(x => x.Role)
                .Include(x => x.CreatedByUser)
                .Include(x => x.AcceptedByUser)
                .FirstOrDefaultAsync(
                    x => x.Token == token, cancellationToken);
        }

        // Get an invitation for a specific event and email address.
        public async Task<PhotographerInvitation?> GetByEventAndEmailAsync(Guid eventId,string email,CancellationToken cancellationToken)
        {
            return await context.PhotographerInvitations
                .Include(x => x.Role)
                .Include(x => x.CreatedByUser)
                .Include(x => x.AcceptedByUser)
                .FirstOrDefaultAsync(
                    x => x.EventId == eventId && x.Email == email.ToLowerInvariant(), cancellationToken);
        }

        // Get all photographer invitations belonging to a specific event.
        public async Task<IReadOnlyList<PhotographerInvitation>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken)
        {
            return await context.PhotographerInvitations
                .Include(x => x.Role)
                .Include(x => x.CreatedByUser)
                .Include(x => x.AcceptedByUser)
                .Where(x => x.EventId == eventId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        // Get only pending photographer invitations for a specific event.
        public async Task<IReadOnlyList<PhotographerInvitation>> GetPendingByEventIdAsync(Guid eventId,
            CancellationToken cancellationToken)
        {
            return await context.PhotographerInvitations
                .Include(x => x.Role)
                .Include(x => x.CreatedByUser)
                .Include(x => x.AcceptedByUser)
                .Where(x =>
                    x.EventId == eventId &&
                    x.Status == InvitationStatus.Pending)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        // Add a new photographer invitation to the database.
        public async Task AddAsync(PhotographerInvitation invitation, CancellationToken cancellationToken)
        {
            await context.PhotographerInvitations.AddAsync(invitation, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        // Update an existing photographer invitation in the database
        public async Task UpdateAsync(PhotographerInvitation invitation, CancellationToken cancellationToken)
        {
            // Mark the invitation as modified in the EF Core change tracker
            context.PhotographerInvitations.Update(invitation);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}