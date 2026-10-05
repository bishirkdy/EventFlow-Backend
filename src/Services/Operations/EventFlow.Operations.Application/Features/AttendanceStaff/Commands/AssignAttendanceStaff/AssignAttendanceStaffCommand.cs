using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Enums;
using MediatR;

namespace EventFlow.Operations.Application.Features.AttendanceStaff.Commands.AssignAttendanceStaff;

public sealed record AssignAttendanceStaffCommand(
    Guid EventId,
    string Email,
    AttendanceScopeType ScopeType,
    Guid? ScopeId) : IRequest<AttendanceStaffDto>;
