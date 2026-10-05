namespace EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakerById;

public sealed record SpeakerSessionResponse(
    Guid Id,
    string Title,
    string SessionType,
    DateTime? StartTime,
    DateTime? EndTime);
