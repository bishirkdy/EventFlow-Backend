using EventFlow.Identity.Domain.Enums;

namespace EventFlow.Identity.Application.Features.EventInvitations.Queries.GetEventInvitation;

public sealed record GetEventInvitationResponse(
    Guid InvitationId,
    Guid EventId,
    string EventName,
    string Email,
    string RoleName,
    EventInvitationType Type,
    InvitationScopeType? ScopeType,
    Guid? ScopeId,
    DateTime ExpiresAt,
    InvitationStatus Status);
