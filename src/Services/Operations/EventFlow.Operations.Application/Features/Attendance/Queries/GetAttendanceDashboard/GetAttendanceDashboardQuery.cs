using EventFlow.Operations.Application.Contracts;
using MediatR;

namespace EventFlow.Operations.Application.Features.Attendance.Queries.GetAttendanceDashboard;

public sealed record GetAttendanceDashboardQuery(
    Guid EventId) : IRequest<AttendanceDashboardDto>;
