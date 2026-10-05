using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Domain.Enums;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.CreatePhotographerInvitation;

public sealed class CreatePhotographerInvitationCommandHandler(
    IPhotographerInvitationRepository invitationRepository,
    IRoleRepository roleRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser)
    : IRequestHandler<CreatePhotographerInvitationCommand, CreatePhotographerInvitationResponse>
{
    public async Task<CreatePhotographerInvitationResponse> Handle(
        CreatePhotographerInvitationCommand request,
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

        var photographerRole = await roleRepository.GetByNameAsync(
            RoleConstants.Photographer,
            cancellationToken);

        if (photographerRole is null)
        {
            throw new ConflictException("The Photographer role is not configured.");
        }

        var existingInvitation = await invitationRepository.GetByEventAndEmailAsync(
            request.EventId,
            request.Email.Trim(),
            cancellationToken);

        if (existingInvitation is not null && existingInvitation.Status == InvitationStatus.Pending)
        {
            throw new ConflictException("A pending invitation already exists for this email and event.");
        }

        var invitation = new PhotographerInvitation(
            request.EventId,
            request.Email.Trim(),
            photographerRole.Id,
            currentUser.UserId);

        await invitationRepository.AddAsync(invitation, cancellationToken);

        return new CreatePhotographerInvitationResponse(
            invitation.Id,
            invitation.Token,
            invitation.ExpiresAt);
    }
}
