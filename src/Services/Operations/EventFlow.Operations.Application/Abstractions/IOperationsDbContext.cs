using EventFlow.Operations.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Abstractions;

public interface IOperationsDbContext
{
    DbSet<AttendanceStaffAssignment> AttendanceStaffAssignments { get; }
    DbSet<AttendanceRecord> AttendanceRecords { get; }
    DbSet<Notification> Notifications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
