using MediatR;
namespace EventFlow.Event.Application.Features.SessionSpeakers.Commands.AssignSpeaker;
public sealed record AssignSpeakerCommand(Guid EventId, Guid SessionId, Guid SpeakerId) : IRequest;
