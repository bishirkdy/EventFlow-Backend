using MediatR;

namespace EventFlow.Operations.Application.Features.AttendanceStaff.Commands.RevokeAttendanceStaff;

public sealed record RevokeAttendanceStaffCommand(
    Guid EventId,
    Guid AssignmentId) : IRequest<object?>;
