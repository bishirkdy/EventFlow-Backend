using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Infrastructure.Persistence;

public sealed class OperationsDbContext(DbContextOptions<OperationsDbContext> options) : DbContext(options), IOperationsDbContext
{
    public DbSet<AttendanceStaffAssignment> AttendanceStaffAssignments => Set<AttendanceStaffAssignment>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<AttendanceStaffAssignment>(b =>
        {
            b.HasKey(x => x.Id);

            b.HasIndex(x => new
            {
                x.EventId,
                x.UserId,
                x.ScopeType,
                x.ScopeId,
                x.IsActive
            })
            .IsUnique();

            b.Property(x => x.ScopeType)
                .HasConversion<int>();
        });

        m.Entity<AttendanceRecord>(b =>
        {
            b.HasKey(x => x.Id);

            b.HasIndex(x => new
            {
                x.EventId,
                x.RegistrationId,
                x.SessionId
            })
            .IsUnique();

            b.Property(x => x.Method)
                .HasConversion<int>();
        });

        m.Entity<Notification>(b =>
        {
            b.HasKey(x => x.Id);

            b.HasIndex(x => new
            {
                x.Status,
                x.ScheduledAtUtc
            });

            b.Property(x => x.Status)
                .HasConversion<int>();

            b.Property(x => x.RecipientEmail)
                .HasMaxLength(320)
                .IsRequired();

            b.Property(x => x.Subject)
                .HasMaxLength(300)
                .IsRequired();
        });
    }
}