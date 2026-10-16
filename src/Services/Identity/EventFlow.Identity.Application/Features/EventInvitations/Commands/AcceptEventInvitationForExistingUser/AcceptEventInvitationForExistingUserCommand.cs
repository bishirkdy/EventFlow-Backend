using EventFlow.Identity.Application.Features.EventInvitations.Commands.AcceptEventInvitation;
using MediatR;

namespace EventFlow.Identity.Application.Features.EventInvitations.Commands.AcceptEventInvitationForExistingUser;

public sealed record AcceptEventInvitationForExistingUserCommand(string Token)
    : IRequest<AcceptEventInvitationResponse>;
