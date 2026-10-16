using MediatR;

namespace EventFlow.Identity.Application.Features.EventInvitations.Commands.RevokeEventInvitation;

public sealed record RevokeEventInvitationCommand(Guid EventId, Guid InvitationId)
    : IRequest;
