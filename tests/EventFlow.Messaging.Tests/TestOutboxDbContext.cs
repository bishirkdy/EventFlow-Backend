using EventFlow.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Messaging.Tests;

public sealed class TestOutboxDbContext(DbContextOptions<TestOutboxDbContext> options)
    : DbContext(options), IOutboxDbContext
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyOutbox();
    }
}
