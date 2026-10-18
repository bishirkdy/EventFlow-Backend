namespace EventFlow.Event.Application.Features.Admin.Queries.GetAdminEvents;

public sealed class AdminEventListItemResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string EventType { get; init; } = default!;
    public string? SubType { get; init; }
    public string Status { get; init; } = default!;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string TimeZone { get; init; } = default!;
    public Guid CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
}
