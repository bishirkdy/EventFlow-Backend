namespace EventFlow.Event.Application.Features.Analytics.Common;

public sealed record SectionCountResponse(Guid SectionId, string Name, int Count);
