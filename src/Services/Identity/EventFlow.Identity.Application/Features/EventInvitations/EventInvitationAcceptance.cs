using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Domain.Enums;
using EventFlow.Messaging.Events;
using EventFlow.Messaging.Outbox;

namespace EventFlow.Identity.Application.Features.EventInvitations;

// Shared acceptance steps used when a photographer, organizer or attendance
// staff invitation is accepted (either with a new account or an existing one).
internal static class EventInvitationAcceptance
{
    public static async Task AssignRoleAsync(
        IUserEventRoleRepository userEventRoleRepository,
        EventInvitation invitation,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var alreadyHasRole = await userEventRoleRepository.ExistsAsync(
            userId,
            invitation.EventId,
            invitation.RoleId,
            cancellationToken);

        if (!alreadyHasRole)
        {
            var userEventRole = new UserEventRole(userId, invitation.EventId, invitation.RoleId);
            await userEventRoleRepository.AddAsync(userEventRole, cancellationToken);
        }
    }

    public static void EnqueueAcceptedIfAttendance(
        IOutbox outbox,
        EventInvitation invitation,
        Guid userId)
    {
        if (invitation.Type != EventInvitationType.AttendanceStaff)
        {
            return;
        }

        outbox.Enqueue("attendance-staff-invitation-accepted",
            new AttendanceStaffInvitationAcceptedEvent
            {
                MessageId = Guid.NewGuid(),
                EventId = invitation.EventId,
                EventContextId = invitation.EventId,
                InvitationId = invitation.Id,
                UserId = userId,
                ScopeType = invitation.ScopeType is null ? null : (int)invitation.ScopeType,
                ScopeId = invitation.ScopeId,
                OccurredAtUtc = DateTime.UtcNow
            });
    }
}
