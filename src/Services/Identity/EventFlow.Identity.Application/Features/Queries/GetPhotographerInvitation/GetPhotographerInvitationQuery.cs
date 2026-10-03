using EventFlow.Identity.Domain.Entities;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetPhotographerInvitation
{
    public sealed record GetPhotographerInvitationQuery(string Token) : IRequest<GetPhotographerInvitationResponse?>;

    public sealed record GetPhotographerInvitationResponse(
        Guid InvitationId,
        Guid EventId,
        string EventName,
        string Email,
        string RoleName,
        DateTime ExpiresAt,
        InvitationStatus Status);
}