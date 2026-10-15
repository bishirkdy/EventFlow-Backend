namespace EventFlow.Identity.Application.Features.PhotographerInvitations.Commands.AcceptPhotographerInvitation;

public sealed record AcceptPhotographerInvitationResponse(
    Guid UserId,
    Guid InvitationId);
