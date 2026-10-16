using EventFlow.Messaging.Events;

namespace EventFlow.Messaging.Outbox;

// Enqueues an integration event into the current unit of work. The caller is
// expected to persist the surrounding DbContext (the outbox row is saved
// together with the business change).
public interface IOutbox
{
    void Enqueue(string topic, IntegrationEvent message);
}
