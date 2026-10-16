namespace EventFlow.Messaging.Correlation;

// Supplies the correlation id of the current operation so outgoing integration
// events can carry it across service boundaries. Returns null when there is no
// ambient correlation (e.g. background work).
public interface ICorrelationIdProvider
{
    string? Get();
}
