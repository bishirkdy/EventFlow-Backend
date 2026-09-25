namespace EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorById;
public sealed record GetSponsorByIdResponse(Guid Id, Guid EventId, string Name, string? Description, string? WebsiteUrl, string? LogoUrl, string SponsorLevel, int DisplayOrder, bool IsActive);
