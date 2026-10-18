namespace EventFlow.Event.Application.Features.Events.Common;

public sealed class GetEventResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public string EventType { get; init; } = default!;
    public string? SubType { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string TimeZone { get; init; } = default!;
    public string Status { get; init; } = default!;
    public IReadOnlyList<EventImageResponse> Images { get; init; } = [];
}
