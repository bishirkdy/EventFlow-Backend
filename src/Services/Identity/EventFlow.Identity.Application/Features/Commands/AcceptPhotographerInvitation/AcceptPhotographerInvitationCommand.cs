using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.AcceptPhotographerInvitation;

public sealed record AcceptPhotographerInvitationCommand(
    string Token,
    string Password,
    string UserName,
    string FirstName,
    string LastName)
    : IRequest<AcceptPhotographerInvitationResponse>;
