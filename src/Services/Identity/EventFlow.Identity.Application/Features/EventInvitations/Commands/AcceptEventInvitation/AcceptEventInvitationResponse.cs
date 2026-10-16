namespace EventFlow.Identity.Application.Features.EventInvitations.Commands.AcceptEventInvitation;

public sealed record AcceptEventInvitationResponse(
    Guid UserId,
    Guid InvitationId);
