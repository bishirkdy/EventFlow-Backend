using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Enums;
using MediatR;

namespace EventFlow.Operations.Application.Features.Attendance.Commands.CheckInQr;

public sealed record CheckInQrCommand(
    Guid EventId,
    string QrCode,
    Guid? SectionId,
    Guid? SessionId,
    AttendanceMethod Method = AttendanceMethod.Qr,
    string? BearerToken = null) : IRequest<AttendanceDto>;
