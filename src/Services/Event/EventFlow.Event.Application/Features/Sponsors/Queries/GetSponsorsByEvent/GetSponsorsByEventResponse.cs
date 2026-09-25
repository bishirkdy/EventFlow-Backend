namespace EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorsByEvent;
public sealed record GetSponsorsByEventResponse(Guid Id, Guid EventId, string Name, string? Description, string? WebsiteUrl, string? LogoUrl, string SponsorLevel, int DisplayOrder, bool IsActive);
