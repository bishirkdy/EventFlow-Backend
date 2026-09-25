using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities;

public sealed class SessionSpeaker : Entity
{
    public Guid SessionId { get; private set; }
    public Guid SpeakerId { get; private set; }
    public Session Session { get; private set; } = null!;
    public Speaker Speaker { get; private set; } = null!;

    private SessionSpeaker() { }

    public SessionSpeaker(Guid sessionId, Guid speakerId)
    {
        Id = Guid.NewGuid();
        SessionId = sessionId;
        SpeakerId = speakerId;
    }
}
