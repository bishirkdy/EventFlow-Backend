using EventFlow.Identity.Domain.Entities;

namespace EventFlow.Identity.Application.Abstractions.Repositories
{
    public interface IPhotographerInvitationRepository
    {
        Task<PhotographerInvitation?> GetByIdAsync(Guid invitationId, CancellationToken cancellationToken);
        Task<PhotographerInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task<PhotographerInvitation?> GetByEventAndEmailAsync(Guid eventId, string email, CancellationToken cancellationToken);
        Task<IReadOnlyList<PhotographerInvitation>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
        Task<IReadOnlyList<PhotographerInvitation>> GetPendingByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
        
        Task AddAsync(PhotographerInvitation invitation, CancellationToken cancellationToken);
        Task UpdateAsync(PhotographerInvitation invitation, CancellationToken cancellationToken);
    }
}