namespace EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakerById;
public sealed record SpeakerSessionResponse(Guid Id, string Title, string SessionType, DateTime? StartTime, DateTime? EndTime);
public sealed record GetSpeakerByIdResponse(Guid Id, Guid EventId, string Name, string? Bio, string? Designation, string? Organization, string? Email, string? ImageUrl, int DisplayOrder, bool IsActive, IReadOnlyList<SpeakerSessionResponse> Sessions);
