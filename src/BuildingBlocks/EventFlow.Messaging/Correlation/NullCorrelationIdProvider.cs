namespace EventFlow.Messaging.Correlation;

public sealed class NullCorrelationIdProvider : ICorrelationIdProvider
{
    public string? Get() => null;
}
