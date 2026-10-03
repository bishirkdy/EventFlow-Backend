using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.CreatePhotographerInvitation
{
    public sealed record CreatePhotographerInvitationCommand(Guid EventId, string Email, Guid CreatedBy, Guid RoleId) : IRequest<CreatePhotographerInvitationResponse>;

    public sealed record CreatePhotographerInvitationResponse(Guid InvitationId, string Token, DateTime ExpiresAt);
}