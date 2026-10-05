using EventFlow.Identity.Domain.Enums;

namespace EventFlow.Identity.Application.Features.Queries.GetPhotographerInvitation;

public sealed record GetPhotographerInvitationResponse(
    Guid InvitationId,
    Guid EventId,
    string EventName,
    string Email,
    string RoleName,
    DateTime ExpiresAt,
    InvitationStatus Status);
