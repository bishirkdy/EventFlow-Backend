namespace EventFlow.Event.Application.Features.SessionSpeakers.Queries.GetSessionSpeakers;
public sealed record GetSessionSpeakersResponse(Guid Id, Guid SpeakerId, string Name, string? Designation, string? Organization, string? ImageUrl);
