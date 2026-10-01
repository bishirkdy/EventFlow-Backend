using MediatR;

namespace EventFlow.Event.Application.Features.Events.Commands.ClaimEventOwner;

public sealed record ClaimEventOwnerCommand(Guid EventId, Guid UserId) : IRequest;
