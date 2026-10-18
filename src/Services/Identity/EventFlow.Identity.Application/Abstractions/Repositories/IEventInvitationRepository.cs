using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Domain.Enums;

namespace EventFlow.Identity.Application.Abstractions.Repositories;

public interface IEventInvitationRepository
{
    Task<EventInvitation?> GetByIdAsync(Guid invitationId, CancellationToken cancellationToken);
    Task<EventInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken);
    Task<EventInvitation?> GetByEventAndEmailAsync(Guid eventId, string email, EventInvitationType type, CancellationToken cancellationToken);
    Task<IReadOnlyList<EventInvitation>> GetByEventIdAsync(Guid eventId, EventInvitationType? type, CancellationToken cancellationToken);
    Task<IReadOnlyList<EventInvitation>> GetPendingByEventIdAsync(Guid eventId, CancellationToken cancellationToken);

    Task AddAsync(EventInvitation invitation, CancellationToken cancellationToken);
    Task UpdateAsync(EventInvitation invitation, CancellationToken cancellationToken);
}
