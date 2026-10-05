using EventFlow.Identity.Domain.Enums;

namespace EventFlow.Identity.Application.Features.Queries.GetEventPhotographerInvitations;

public sealed record GetEventPhotographerInvitationsResponse(
    Guid InvitationId,
    string Email,
    string RoleName,
    InvitationStatus Status,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    DateTime? AcceptedAt,
    Guid? AcceptedByUserId);
