using EventFlow.Messaging.Events;

namespace EventFlow.Messaging.Outbox;

// Durable record of an integration event that must be published to Kafka.
// It is written in the same transaction as the business change so a crash
// cannot leave the database and the broker out of sync.
public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public Guid MessageId { get; private set; }
    public Guid EventContextId { get; private set; }
    public string Topic { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public string? CorrelationId { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
    public OutboxStatus Status { get; private set; }
    public int Attempts { get; private set; }
    public DateTime? LastAttemptAtUtc { get; private set; }
    public DateTime? NextAttemptAtUtc { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    public DateTime? LockedAtUtc { get; private set; }
    public string? Error { get; private set; }

    private OutboxMessage()
    {
    }

    public OutboxMessage(string topic, string payload, IntegrationEvent message)
    {
        Id = Guid.NewGuid();
        MessageId = message.MessageId == Guid.Empty ? Guid.NewGuid() : message.MessageId;
        EventContextId = message.EventContextId;
        Topic = topic;
        Payload = payload;
        CorrelationId = message.CorrelationId;
        OccurredAtUtc = message.OccurredAtUtc == default ? DateTime.UtcNow : message.OccurredAtUtc;
        Status = OutboxStatus.Pending;
    }

    public void MarkProcessing(DateTime now)
    {
        Status = OutboxStatus.Processing;
        LockedAtUtc = now;
        LastAttemptAtUtc = now;
        Attempts++;
    }

    public void MarkProcessed(DateTime now)
    {
        Status = OutboxStatus.Processed;
        ProcessedAtUtc = now;
        LockedAtUtc = null;
        Error = null;
    }

    public void ScheduleRetry(string error, DateTime nextAttemptAtUtc)
    {
        Status = OutboxStatus.Pending;
        Error = error;
        NextAttemptAtUtc = nextAttemptAtUtc;
        LockedAtUtc = null;
    }

    public void MarkFailed(string error)
    {
        Status = OutboxStatus.Failed;
        Error = error;
        LockedAtUtc = null;
    }
}
