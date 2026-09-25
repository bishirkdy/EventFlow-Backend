namespace EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakersByEvent;
public sealed record GetSpeakersByEventResponse(Guid Id, Guid EventId, string Name, string? Bio, string? Designation, string? Organization, string? Email, string? ImageUrl, int DisplayOrder, bool IsActive);
