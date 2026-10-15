using MediatR;

namespace EventFlow.Identity.Application.Features.PhotographerInvitations.Commands.CreatePhotographerInvitation;

public sealed record CreatePhotographerInvitationCommand(Guid EventId, string Email)
    : IRequest<CreatePhotographerInvitationResponse>;
