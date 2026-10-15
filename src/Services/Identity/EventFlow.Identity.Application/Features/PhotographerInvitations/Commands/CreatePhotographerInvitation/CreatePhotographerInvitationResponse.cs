namespace EventFlow.Identity.Application.Features.PhotographerInvitations.Commands.CreatePhotographerInvitation;

public sealed record CreatePhotographerInvitationResponse(
    Guid InvitationId,
    string Token,
    DateTime ExpiresAt);
