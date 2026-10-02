using EventFlow.Operations.Domain.Enums;

namespace EventFlow.Operations.Application.Contracts;

public sealed record AttendanceStaffDto(
    Guid Id,
    Guid EventId,
    Guid UserId,
    AttendanceScopeType ScopeType,
    Guid? ScopeId,
    bool IsActive);
