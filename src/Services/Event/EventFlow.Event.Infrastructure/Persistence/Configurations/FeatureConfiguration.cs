

using EventFlow.Event.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Event.Infrastructure.Persistence.Configurations
{
    public sealed class FeatureConfiguration : IEntityTypeConfiguration<Feature>
    {
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            builder.ToTable("Features");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Code)
                .HasMaxLength(100)
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

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.HasData(
                new
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    Code = "schedule",
                    Name = "Schedule",
                    Description = "Manage the event schedule.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    Code = "sessions",
                    Name = "Sessions",
                    Description = "Manage event sessions.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    Code = "venues",
                    Name = "Venues",
                    Description = "Manage event venues.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    Code = "speakers",
                    Name = "Speakers",
                    Description = "Manage event speakers.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                    Code = "sponsors",
                    Name = "Sponsors",
                    Description = "Manage event sponsors.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                    Code = "registration",
                    Name = "Registration",
                    Description = "Manage participant registration.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000007"),
                    Code = "attendance",
                    Name = "Attendance",
                    Description = "Manage participant attendance.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000008"),
                    Code = "gallery",
                    Name = "Gallery",
                    Description = "Manage event photos and gallery.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000009"),
                    Code = "certificates",
                    Name = "Certificates",
                    Description = "Manage event certificates.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000010"),
                    Code = "feedback",
                    Name = "Feedback",
                    Description = "Collect participant feedback.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
