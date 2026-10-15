using MediatR;

namespace EventFlow.Identity.Application.Features.PhotographerInvitations.Commands.RevokePhotographerInvitation;

public sealed record RevokePhotographerInvitationCommand(Guid EventId, Guid InvitationId)
    : IRequest;
