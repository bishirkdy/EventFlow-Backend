namespace EventFlow.Identity.Application.Features.Commands.CreatePhotographerInvitation;

public sealed record CreatePhotographerInvitationResponse(
    Guid InvitationId,
    string Token,
    DateTime ExpiresAt);
