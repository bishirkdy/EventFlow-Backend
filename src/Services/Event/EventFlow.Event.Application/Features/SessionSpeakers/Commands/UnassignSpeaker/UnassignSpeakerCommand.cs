using MediatR;
namespace EventFlow.Event.Application.Features.SessionSpeakers.Commands.UnassignSpeaker;
public sealed record UnassignSpeakerCommand(Guid EventId, Guid SessionId, Guid SpeakerId) : IRequest;
