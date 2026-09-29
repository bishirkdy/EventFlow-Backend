namespace EventFlow.Registration.Infrastructure.Persistence.Configurations;

public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> b)
    {
        b.ToTable("Tickets");
        b.HasKey(x => x.Id);
        b.Property(x => x.TicketNumber).HasMaxLength(60).IsRequired();
        b.HasIndex(x => x.TicketNumber).IsUnique();
        b.Property(x => x.QrCodeValue).HasMaxLength(500).IsRequired();
        b.HasIndex(x => x.QrCodeValue).IsUnique();

        b.HasOne(x => x.Participant)
            .WithOne(x => x.Ticket)
            .HasForeignKey<Ticket>(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
