using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetPhotographerInvitation;

public sealed record GetPhotographerInvitationQuery(string Token)
    : IRequest<GetPhotographerInvitationResponse>;
