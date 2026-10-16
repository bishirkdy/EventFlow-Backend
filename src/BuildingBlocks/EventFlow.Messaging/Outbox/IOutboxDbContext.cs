using Microsoft.EntityFrameworkCore;

namespace EventFlow.Messaging.Outbox;

// Exposes the outbox table on a service DbContext so a business change and its
// outgoing integration events can be persisted in a single SaveChanges call.
public interface IOutboxDbContext
{
    DbSet<OutboxMessage> OutboxMessages { get; }
}
