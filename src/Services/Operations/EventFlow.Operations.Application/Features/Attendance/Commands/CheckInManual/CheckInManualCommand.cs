using EventFlow.Operations.Application.Contracts;
using MediatR;

namespace EventFlow.Operations.Application.Features.Attendance.Commands.CheckInManual;

public sealed record CheckInManualCommand(
    Guid EventId,
    Guid RegistrationId,
    Guid? SectionId,
    Guid? SessionId) : IRequest<AttendanceDto>;
