

namespace EventFlow.Messaging.Events
{
    public abstract record IntegrationEvent
    {
        public Guid EventId { get; init; }
        public Guid EventContextId { get; init; }
        public DateTime OccurredAtUtc { get; init; }
    }
}
