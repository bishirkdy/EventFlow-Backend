using EventFlow.Messaging.Correlation;
using EventFlow.Messaging.Events;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EventFlow.Messaging.Outbox;

internal sealed class Outbox<TContext>(TContext context, ICorrelationIdProvider correlationIdProvider) : IOutbox
    where TContext : DbContext, IOutboxDbContext
{
    public void Enqueue(string topic, IntegrationEvent message)
    {
        var correlationId = message.CorrelationId ?? correlationIdProvider.Get();

        var envelope = correlationId is null
            ? message
            : message with { CorrelationId = correlationId };

        // Serialize using the runtime type so the derived event properties are kept.
        var payload = JsonSerializer.Serialize(envelope, envelope.GetType());

        context.OutboxMessages.Add(new OutboxMessage(topic, payload, envelope));
    }
}
