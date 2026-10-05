namespace EventFlow.Event.Application.Features.Analytics.Common;

public sealed record PageSectionCountResponse(Guid PageId, string PageName, int Count);
