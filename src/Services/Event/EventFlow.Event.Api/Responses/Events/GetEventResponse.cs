namespace EventFlow.Event.Api.Responses.Events
{
    public sealed class GetEventResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
        public string EventType { get; init; } = default!;
        public string? SubType { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public string Timezone { get; init; } = default!;
        public string Status { get; init; } = default!;
        public string? Subdomain { get; init; }
        public Guid CreatedBy { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
