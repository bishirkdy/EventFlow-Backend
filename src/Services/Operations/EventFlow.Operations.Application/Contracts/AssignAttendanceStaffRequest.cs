using EventFlow.Operations.Domain.Enums;

namespace EventFlow.Operations.Application.Contracts;

public sealed record AssignAttendanceStaffRequest(
    string Email,
    AttendanceScopeType ScopeType,
    Guid? ScopeId);
