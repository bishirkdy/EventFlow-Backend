using EventFlow.Operations.Application.Contracts;
using MediatR;

namespace EventFlow.Operations.Application.Features.AttendanceStaff.Queries.GetAttendanceStaff;

public sealed record GetAttendanceStaffQuery(
    Guid EventId) : IRequest<IReadOnlyList<AttendanceStaffDto>>;
