

namespace EventFlow.Messaging.Events;

public abstract record IntegrationEvent
{
    public Guid MessageId { get; init; }
    public Guid EventId { get; init; }
    public Guid EventContextId { get; init; }
    public string? CorrelationId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
}
