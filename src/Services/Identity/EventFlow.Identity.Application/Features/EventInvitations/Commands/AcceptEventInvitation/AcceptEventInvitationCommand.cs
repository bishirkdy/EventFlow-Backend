using MediatR;

namespace EventFlow.Identity.Application.Features.EventInvitations.Commands.AcceptEventInvitation;

public sealed record AcceptEventInvitationCommand(
    string Token,
    string Password,
    string UserName,
    string FirstName,
    string LastName)
    : IRequest<AcceptEventInvitationResponse>;
