namespace EventFlow.Messaging.Outbox;

public sealed class OutboxOptions
{
    public const string SectionName = "Outbox";

    public int BatchSize { get; set; } = 50;

    public int MaxAttempts { get; set; } = 5;

    public int PollIntervalSeconds { get; set; } = 5;

    public int RetryBackoffSeconds { get; set; } = 10;

    public int LockTimeoutSeconds { get; set; } = 120;
}
