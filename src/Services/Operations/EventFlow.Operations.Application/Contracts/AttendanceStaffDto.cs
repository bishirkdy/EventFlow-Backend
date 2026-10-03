using EventFlow.Operations.Domain.Enums;
namespace EventFlow.Operations.Application.Contracts;
public sealed record AttendanceStaffDto(Guid Id,Guid EventId,Guid UserId,string Email,AttendanceScopeType ScopeType,Guid? ScopeId,bool IsActive);
