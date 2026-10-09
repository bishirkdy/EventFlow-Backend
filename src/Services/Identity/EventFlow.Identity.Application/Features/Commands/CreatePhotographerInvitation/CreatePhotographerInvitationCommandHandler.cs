using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Domain.Enums;
using EventFlow.Messaging.Events;
using EventFlow.Messaging.Kafka;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.CreatePhotographerInvitation;

public sealed class CreatePhotographerInvitationCommandHandler(
    IPhotographerInvitationRepository invitationRepository,
    IRoleRepository roleRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser,
    IKafkaProducer kafkaProducer)
    : IRequestHandler<CreatePhotographerInvitationCommand, CreatePhotographerInvitationResponse>
{
    public async Task<CreatePhotographerInvitationResponse> Handle(
        CreatePhotographerInvitationCommand request,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(currentUser.UserId, request.EventId, PermissionConstants.Event.TeamManage,
                cancellationToken))
        {
            throw new ForbiddenException(
                "You do not have permission to manage the event team.");
        }

        var photographerRole = await roleRepository.GetByNameAsync(RoleConstants.Photographer, cancellationToken);

        if (photographerRole is null)
        {
            throw new ConflictException(
                "The Photographer role is not configured.");
        }

        var email = request.Email.Trim();

        var existingInvitation =
            await invitationRepository.GetByEventAndEmailAsync(request.EventId,email, cancellationToken);

        if (existingInvitation is not null && existingInvitation.Status == InvitationStatus.Pending)
        {
            throw new ConflictException("A pending invitation already exists for this email and event.");
        }

        var invitation = new PhotographerInvitation(request.EventId, email, photographerRole.Id, currentUser.UserId);

        await invitationRepository.AddAsync(invitation, cancellationToken);

        await kafkaProducer.PublishAsync("photographer-invitation-created",
            new PhotographerInvitationCreatedEvent
            {
                EventId = invitation.EventId,
                EventContextId = invitation.EventId,
                InvitationId = invitation.Id,
                Email = invitation.Email,
                Token = invitation.Token,
                ExpiresAt = invitation.ExpiresAt,
                OccurredAtUtc = DateTime.UtcNow
            },
            cancellationToken);

        return new CreatePhotographerInvitationResponse(
            invitation.Id,
            invitation.Token,
            invitation.ExpiresAt);
    }
}