namespace EventFlow.Messaging.Kafka;

public sealed class KafkaOptions
{
    public string BootstrapServers { get; set; } = string.Empty;

    public int ConsumerMaxAttempts { get; set; } = 3;

    public int ConsumerRetryBackoffSeconds { get; set; } = 5;

    public string DeadLetterSuffix { get; set; } = ".dlq";
}
