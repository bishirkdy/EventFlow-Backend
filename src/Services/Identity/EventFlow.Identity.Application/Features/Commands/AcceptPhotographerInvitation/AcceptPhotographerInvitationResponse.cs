namespace EventFlow.Identity.Application.Features.Commands.AcceptPhotographerInvitation;

public sealed record AcceptPhotographerInvitationResponse(
    Guid UserId,
    Guid InvitationId);
