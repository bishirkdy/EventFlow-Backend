namespace EventFlow.Event.Application.Features.Analytics.Common;

public sealed record KeyCountResponse(string Key, int Count);

public sealed record DayCountResponse(string Date, int Count);

public sealed record SectionCountResponse(Guid SectionId, string Name, int Count);

public sealed record VenueLoadResponse(Guid VenueId, string Name, int Capacity, int SessionCount);

public sealed record PageSectionCountResponse(Guid PageId, string PageName, int Count);
