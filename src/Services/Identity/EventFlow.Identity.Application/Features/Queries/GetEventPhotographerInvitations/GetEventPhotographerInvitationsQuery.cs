using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetEventPhotographerInvitations;

public sealed record GetEventPhotographerInvitationsQuery(Guid EventId)
    : IRequest<IReadOnlyList<GetEventPhotographerInvitationsResponse>>;
