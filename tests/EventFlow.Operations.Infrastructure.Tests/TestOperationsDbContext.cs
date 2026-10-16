using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Infrastructure.Tests;

public sealed class TestOperationsDbContext(
    DbContextOptions<TestOperationsDbContext> options,
    FailureController failures) : DbContext(options), IOperationsDbContext
{
    public DbSet<AttendanceStaffAssignment> AttendanceStaffAssignments => Set<AttendanceStaffAssignment>();

    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

    public int SaveCalls { get; private set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCalls++;

        if (failures.ShouldFail())
        {
            throw new InvalidOperationException("Database failure.");
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
