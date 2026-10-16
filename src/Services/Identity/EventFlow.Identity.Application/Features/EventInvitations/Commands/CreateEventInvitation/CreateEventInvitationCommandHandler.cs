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

namespace EventFlow.Identity.Application.Features.EventInvitations.Commands.CreateEventInvitation;

public sealed class CreateEventInvitationCommandHandler(
    IEventInvitationRepository invitationRepository,
    IRoleRepository roleRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser,
    IKafkaProducer kafkaProducer)
    : IRequestHandler<CreateEventInvitationCommand, CreateEventInvitationResponse>
{
    public async Task<CreateEventInvitationResponse> Handle(
        CreateEventInvitationCommand request,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(currentUser.UserId, request.EventId, PermissionConstants.Event.TeamManage,
                cancellationToken))
        {
            throw new ForbiddenException(
                "You do not have permission to manage the event team.");
        }

        var roleName = ResolveRoleName(request.Type);

        var role = await roleRepository.GetByNameAsync(roleName, cancellationToken);

        if (role is null)
        {
            throw new ConflictException(
                $"The {roleName} role is not configured.");
        }

        var email = request.Email.Trim();

        var existingInvitation =
            await invitationRepository.GetByEventAndEmailAsync(request.EventId, email, request.Type, cancellationToken);

        if (existingInvitation is not null && existingInvitation.Status == InvitationStatus.Pending)
        {
            throw new ConflictException("A pending invitation already exists for this email and event.");
        }

        var invitation = new EventInvitation(
            request.EventId,
            email,
            role.Id,
            currentUser.UserId,
            request.Type,
            request.ScopeType,
            request.ScopeId);

        await invitationRepository.AddAsync(invitation, cancellationToken);

        await kafkaProducer.PublishAsync("event-invitation-created",
            new EventInvitationCreatedEvent
            {
                EventId = invitation.EventId,
                EventContextId = invitation.EventId,
                InvitationId = invitation.Id,
                Email = invitation.Email,
                InvitationType = (int)invitation.Type,
                ScopeType = invitation.ScopeType is null ? null : (int)invitation.ScopeType,
                ScopeId = invitation.ScopeId,
                Token = invitation.Token,
                ExpiresAt = invitation.ExpiresAt,
                OccurredAtUtc = DateTime.UtcNow
            },
            cancellationToken);

        return new CreateEventInvitationResponse(
            invitation.Id,
            invitation.Token,
            invitation.ExpiresAt);
    }

    private static string ResolveRoleName(EventInvitationType type) => type switch
    {
        EventInvitationType.Photographer => RoleConstants.Photographer,
        EventInvitationType.Organizer => RoleConstants.Organizer,
        EventInvitationType.AttendanceStaff => RoleConstants.AttendanceStaff,
        _ => throw new ConflictException("The invitation type is not supported.")
    };
}
