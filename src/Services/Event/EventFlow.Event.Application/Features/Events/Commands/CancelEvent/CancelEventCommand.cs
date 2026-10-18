using MediatR;

namespace EventFlow.Event.Application.Features.Events.Commands.CancelEvent;

public sealed record CancelEventCommand(Guid Id) : IRequest;
