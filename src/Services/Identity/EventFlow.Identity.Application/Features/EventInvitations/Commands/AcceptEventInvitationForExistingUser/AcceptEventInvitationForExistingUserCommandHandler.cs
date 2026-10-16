using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Features.EventInvitations.Commands.AcceptEventInvitation;
using EventFlow.Identity.Domain.Enums;
using EventFlow.Messaging.Outbox;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.EventInvitations.Commands.AcceptEventInvitationForExistingUser;

public sealed class AcceptEventInvitationForExistingUserCommandHandler(
    IEventInvitationRepository invitationRepository,
    IUserRepository userRepository,
    IUserEventRoleRepository userEventRoleRepository,
    ICurrentUserService currentUser,
    IOutbox outbox)
    : IRequestHandler<AcceptEventInvitationForExistingUserCommand, AcceptEventInvitationResponse>
{
    public async Task<AcceptEventInvitationResponse> Handle(
        AcceptEventInvitationForExistingUserCommand request,
        CancellationToken cancellationToken)
    {
        var invitation = await invitationRepository.GetByTokenAsync(request.Token, cancellationToken);

        if (invitation is null)
        {
            throw new NotFoundException("Invalid invitation token.");
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            throw new ConflictException("Invitation has already been processed or revoked.");
        }

        if (invitation.IsExpired)
        {
            throw new ConflictException("Invitation has expired.");
        }

        var user = await userRepository.GetByIdAsync(currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException("The signed-in user could not be found.");

        if (!string.Equals(
                user.Email.Trim().ToLowerInvariant(),
                invitation.Email.Trim().ToLowerInvariant(),
                StringComparison.Ordinal))
        {
            throw new ForbiddenException(
                "Please sign in with the email address that received this invitation.");
        }

        await EventInvitationAcceptance.AssignRoleAsync(
            userEventRoleRepository,
            invitation,
            user.Id,
            cancellationToken);

        invitation.Accept(user.Id);
        EventInvitationAcceptance.EnqueueAcceptedIfAttendance(outbox, invitation, user.Id);
        await invitationRepository.UpdateAsync(invitation, cancellationToken);

        return new AcceptEventInvitationResponse(user.Id, invitation.Id);
    }
}
