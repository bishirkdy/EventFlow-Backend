using MediatR;

namespace EventFlow.Identity.Application.Features.PhotographerInvitations.Queries.GetEventPhotographerInvitations;

public sealed record GetEventPhotographerInvitationsQuery(Guid EventId)
    : IRequest<IReadOnlyList<GetEventPhotographerInvitationsResponse>>;
