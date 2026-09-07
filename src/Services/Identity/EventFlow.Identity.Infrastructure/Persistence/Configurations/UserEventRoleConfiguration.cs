using EventFlow.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Identity.Infrastructure.Persistence.Configurations
{
    public class UserEventRoleConfiguration : IEntityTypeConfiguration<UserEventRole>
    {
        public void Configure(EntityTypeBuilder<UserEventRole> builder)
        {
            builder.ToTable("UserEventRoles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AssignedAt)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserEventRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Role)
                .WithMany(x => x.UserEventRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // A user should not have the same role twice for the same event.
            builder.HasIndex(x => new
            {
                x.UserId,
                x.EventId,
                x.RoleId
            })
            .IsUnique();
        }
    }
}
