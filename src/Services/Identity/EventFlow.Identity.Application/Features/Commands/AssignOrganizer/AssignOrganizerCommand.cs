using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.AssignOrganizer;

public sealed record AssignOrganizerCommand(Guid EventId, string Email)
    : IRequest<Guid>;
