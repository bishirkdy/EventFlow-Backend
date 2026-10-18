using MediatR;

namespace EventFlow.Event.Application.Features.Events.Commands.PublishEvent;

public sealed record PublishEventCommand(Guid Id) : IRequest;
