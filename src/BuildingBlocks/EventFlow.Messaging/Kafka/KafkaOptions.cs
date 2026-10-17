namespace EventFlow.Messaging.Kafka;

public sealed class KafkaOptions
{
    public string BootstrapServers { get; set; } = string.Empty;

    // Delivery is bounded so a stuck broker fails fast instead of blocking the
    // outbox loop for the client library default (5 minutes) per message.
    public int ProducerMessageTimeoutMs { get; set; } = 15000;

    public int ProducerSocketTimeoutMs { get; set; } = 10000;

    public int ProducerRequestTimeoutMs { get; set; } = 10000;

    public int ConsumerMaxAttempts { get; set; } = 3;

    public int ConsumerRetryBackoffSeconds { get; set; } = 5;

    // How long a consumer may stay without a partition assignment before it is
    // rebuilt. A broker restart can leave a consumer revoked and never
    // rejoined; recreating it re-establishes group membership so lagged
    // messages are picked up again.
    public int ConsumerUnassignedRebuildSeconds { get; set; } = 60;

    // Delay before a rebuilt consumer is created.
    public int ConsumerRebuildBackoffSeconds { get; set; } = 5;

    public string DeadLetterSuffix { get; set; } = ".dlq";
}
