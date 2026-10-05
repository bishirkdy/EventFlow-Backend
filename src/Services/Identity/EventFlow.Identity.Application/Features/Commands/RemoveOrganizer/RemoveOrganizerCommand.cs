using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.RemoveOrganizer;

public sealed record RemoveOrganizerCommand(Guid EventId, Guid UserId)
    : IRequest;
