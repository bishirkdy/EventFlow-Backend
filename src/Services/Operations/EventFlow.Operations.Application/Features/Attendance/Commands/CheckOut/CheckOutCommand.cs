using EventFlow.Operations.Application.Contracts;
using MediatR;

namespace EventFlow.Operations.Application.Features.Attendance.Commands.CheckOut;

public sealed record CheckOutCommand(
    Guid EventId,
    Guid AttendanceId) : IRequest<AttendanceDto>;
