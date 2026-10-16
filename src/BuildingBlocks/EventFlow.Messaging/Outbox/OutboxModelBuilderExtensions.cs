using Microsoft.EntityFrameworkCore;

namespace EventFlow.Messaging.Outbox;

public static class OutboxModelBuilderExtensions
{
    public static ModelBuilder ApplyOutbox(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        return modelBuilder;
    }
}
