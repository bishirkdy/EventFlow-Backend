using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.RevokePhotographerInvitation;

public sealed class RevokePhotographerInvitationCommandHandler(
    IPhotographerInvitationRepository invitationRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser)
    : IRequestHandler<RevokePhotographerInvitationCommand>
{
    public async Task Handle(
        RevokePhotographerInvitationCommand request,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(
                currentUser.UserId,
                request.EventId,
                PermissionConstants.Event.TeamManage,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission to manage the event team.");
        }

        var invitation = await invitationRepository.GetByIdAsync(
            request.InvitationId,
            cancellationToken);

        if (invitation is null)
        {
            throw new NotFoundException("Invitation not found.");
        }

        invitation.Revoke();
        await invitationRepository.UpdateAsync(invitation, cancellationToken);
    }
}
