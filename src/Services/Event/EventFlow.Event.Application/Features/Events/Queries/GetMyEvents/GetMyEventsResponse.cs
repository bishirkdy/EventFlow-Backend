using EventFlow.Event.Application.Features.Events.Common;

namespace EventFlow.Event.Application.Features.Events.Queries.GetMyEvents
{
    public sealed class GetMyEventsResponse
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
        public string? Subdomain { get; init; }
        public Guid CreatedBy { get; init; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public IReadOnlyList<EventImageResponse> Images { get; init; } = [];
    }
}
