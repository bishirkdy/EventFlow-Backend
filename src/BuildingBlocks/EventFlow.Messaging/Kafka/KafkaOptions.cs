namespace EventFlow.Messaging.Kafka;

public sealed class KafkaOptions
{
    public string BootstrapServers { get; set; } = string.Empty;

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
