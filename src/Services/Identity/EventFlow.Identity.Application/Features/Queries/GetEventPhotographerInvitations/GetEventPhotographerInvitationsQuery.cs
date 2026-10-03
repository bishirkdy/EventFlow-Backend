using EventFlow.Identity.Domain.Entities;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetEventPhotographerInvitations
{
    public sealed record GetEventPhotographerInvitationsQuery(Guid EventId) : IRequest<IReadOnlyList<GetEventPhotographerInvitationsResponse>>;

    public sealed record GetEventPhotographerInvitationsResponse(
        Guid InvitationId,
        string Email,
        string RoleName,
        InvitationStatus Status,
        DateTime CreatedAt,
        DateTime ExpiresAt,
        DateTime? AcceptedAt,
        Guid? AcceptedByUserId);
}