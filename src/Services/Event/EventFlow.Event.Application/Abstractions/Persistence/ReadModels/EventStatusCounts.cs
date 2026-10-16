namespace EventFlow.Event.Application.Abstractions.Persistence.ReadModels;

// Aggregate event counts grouped by status for the platform admin dashboard.
public sealed record EventStatusCounts(
    int TotalEvents,
    int PublishedEvents,
    int DraftEvents,
    int CancelledEvents);
