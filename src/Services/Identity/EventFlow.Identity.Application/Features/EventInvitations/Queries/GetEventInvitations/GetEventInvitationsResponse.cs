using EventFlow.Identity.Domain.Enums;

namespace EventFlow.Identity.Application.Features.EventInvitations.Queries.GetEventInvitations;

public sealed record GetEventInvitationsResponse(
    Guid InvitationId,
    string Email,
    string RoleName,
    EventInvitationType Type,
    InvitationScopeType? ScopeType,
    Guid? ScopeId,
    InvitationStatus Status,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    DateTime? AcceptedAt,
    Guid? AcceptedByUserId);
