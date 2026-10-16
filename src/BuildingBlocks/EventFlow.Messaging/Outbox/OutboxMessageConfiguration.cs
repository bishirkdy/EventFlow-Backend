using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Messaging.Outbox;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Topic)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Payload)
            .IsRequired();

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(64);

        builder.Property(x => x.Error)
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.HasIndex(x => x.MessageId)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.Status,
            x.NextAttemptAtUtc
        });
    }
}
