using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.RevokePhotographerInvitation
{
    public sealed record RevokePhotographerInvitationCommand(Guid InvitationId, Guid RevokedBy) : IRequest;
}