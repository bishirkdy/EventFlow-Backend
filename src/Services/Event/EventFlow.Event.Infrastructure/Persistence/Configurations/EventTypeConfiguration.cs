

using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class EventTypeConfiguration: IEntityTypeConfiguration<EventType>
    {
        public void Configure(EntityTypeBuilder<EventType> builder)
        {
            builder.ToTable("EventTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Code)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .IsRequired();

            //Basic seeding
            builder.HasData(
       new
       {
           Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
           Code = EventTypeCode.Wedding,
           Name = "Wedding & Private Events",
           Description = "Weddings and private events",
           IsActive = true,
           CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
       },
       new
       {
           Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
           Code = EventTypeCode.Conference,
           Name = "Conference & Business",
           Description = "Conferences, business events and professional gatherings",
           IsActive = true,
           CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
       },
       new
       {
           Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
           Code = EventTypeCode.Education,
           Name = "Education & Workshop",
           Description = "Educational events, workshops and training programs",
           IsActive = true,
           CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
       },
       new
       {
           Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
           Code = EventTypeCode.Festival,
           Name = "Festival & Cultural",
           Description = "Festivals and cultural events",
           IsActive = true,
           CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
       },
       new
       {
           Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
           Code = EventTypeCode.Sports,
           Name = "Sports & Competition",
           Description = "Sports events and competitions",
           IsActive = true,
           CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
       }
   );
        }
    }
}
