using EventFlow.SharedKernel.Domain;

namespace EventFlow.Operations.Domain.Entities;

// Durable record that a consumer already handled a given message, used to keep
// duplicate Kafka deliveries from repeating business side effects.
public sealed class ProcessedMessage : Entity
{
    // EF Core materialization constructor.
    private ProcessedMessage()
    {
    }

    public ProcessedMessage(string consumer, Guid messageId, DateTime processedAtUtc)
    {
        Consumer = consumer;
        MessageId = messageId;
        ProcessedAtUtc = processedAtUtc;
    }

    public string Consumer { get; private set; } = string.Empty;

    public Guid MessageId { get; private set; }

    public DateTime ProcessedAtUtc { get; private set; }
}
