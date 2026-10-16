using System.Text.Json;
using EventFlow.Messaging.Correlation;
using EventFlow.Messaging.Events;
using Microsoft.EntityFrameworkCore;

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

        var payload = JsonSerializer.Serialize(envelope);

        context.OutboxMessages.Add(new OutboxMessage(topic, payload, envelope));
    }
}
