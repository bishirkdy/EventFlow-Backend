using MediatR;

namespace EventFlow.Identity.Application.Features.PhotographerInvitations.Queries.GetPhotographerInvitation;

public sealed record GetPhotographerInvitationQuery(string Token)
    : IRequest<GetPhotographerInvitationResponse>;
