using MediatR;

namespace EventFlow.Identity.Application.Features.EventInvitations.Queries.GetEventInvitation;

public sealed record GetEventInvitationQuery(string Token)
    : IRequest<GetEventInvitationResponse>;
