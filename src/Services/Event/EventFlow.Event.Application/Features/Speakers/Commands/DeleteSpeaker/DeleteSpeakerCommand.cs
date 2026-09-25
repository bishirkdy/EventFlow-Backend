using MediatR;
namespace EventFlow.Event.Application.Features.Speakers.Commands.DeleteSpeaker;
public sealed record DeleteSpeakerCommand(Guid Id, Guid EventId) : IRequest;
