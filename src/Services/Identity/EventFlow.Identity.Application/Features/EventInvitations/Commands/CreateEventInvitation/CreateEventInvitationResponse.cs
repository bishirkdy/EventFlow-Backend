namespace EventFlow.Identity.Application.Features.EventInvitations.Commands.CreateEventInvitation;

public sealed record CreateEventInvitationResponse(
    Guid InvitationId,
    string Token,
    DateTime ExpiresAt);
