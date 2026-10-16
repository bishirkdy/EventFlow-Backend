using EventFlow.Identity.Domain.Enums;
using MediatR;

namespace EventFlow.Identity.Application.Features.EventInvitations.Commands.CreateEventInvitation;

public sealed record CreateEventInvitationCommand(
    Guid EventId,
    string Email,
    EventInvitationType Type,
    InvitationScopeType? ScopeType,
    Guid? ScopeId)
    : IRequest<CreateEventInvitationResponse>;
