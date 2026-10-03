using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Identity.Infrastructure.Persistence.Repositories
{
    public sealed class PhotographerInvitationRepository(IdentityDbContext context) : IPhotographerInvitationRepository
    {
        public async Task<PhotographerInvitation?> GetByIdAsync(Guid invitationId, CancellationToken cancellationToken)
        {
            return await context.PhotographerInvitations
                .Include(x => x.Role)
                .Include(x => x.CreatedByUser)
                .Include(x => x.AcceptedByUser)
                .FirstOrDefaultAsync(x => x.Id == invitationId, cancellationToken);
        }

        public async Task<PhotographerInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await context.PhotographerInvitations
                .Include(x => x.Role)
                .Include(x => x.CreatedByUser)
                .Include(x => x.AcceptedByUser)
                .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
        }

        public async Task<PhotographerInvitation?> GetByEventAndEmailAsync(Guid eventId, string email, CancellationToken cancellationToken)
        {
            return await context.PhotographerInvitations
                .Include(x => x.Role)
                .Include(x => x.CreatedByUser)
                .Include(x => x.AcceptedByUser)
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.Email == email.ToLowerInvariant(), cancellationToken);
        }

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

        public async Task<IReadOnlyList<PhotographerInvitation>> GetPendingByEventIdAsync(Guid eventId, CancellationToken cancellationToken)
        {
            return await context.PhotographerInvitations
                .Include(x => x.Role)
                .Include(x => x.CreatedByUser)
                .Include(x => x.AcceptedByUser)
                .Where(x => x.EventId == eventId && x.Status == InvitationStatus.Pending)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(PhotographerInvitation invitation, CancellationToken cancellationToken)
        {
            await context.PhotographerInvitations.AddAsync(invitation, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(PhotographerInvitation invitation, CancellationToken cancellationToken)
        {
            context.PhotographerInvitations.Update(invitation);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}