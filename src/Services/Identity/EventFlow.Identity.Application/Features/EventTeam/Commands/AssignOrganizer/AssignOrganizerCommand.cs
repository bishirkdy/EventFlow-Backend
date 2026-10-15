using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Commands.AssignOrganizer;

public sealed record AssignOrganizerCommand(Guid EventId, string Email)
    : IRequest<Guid>;
