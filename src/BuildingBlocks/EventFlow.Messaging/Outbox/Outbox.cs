using System.Text.Json;
using EventFlow.Messaging.Events;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Messaging.Outbox;

internal sealed class Outbox<TContext>(TContext context) : IOutbox
    where TContext : DbContext, IOutboxDbContext
{
    public void Enqueue(string topic, IntegrationEvent message)
    {
        var payload = JsonSerializer.Serialize(message);

        context.OutboxMessages.Add(new OutboxMessage(topic, payload, message));
    }
}
