namespace EventFlow.Event.Application.Features.Analytics.Common;

public sealed record VenueLoadResponse(Guid VenueId, string Name, int Capacity, int SessionCount);
