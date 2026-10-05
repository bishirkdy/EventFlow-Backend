using EventFlow.Operations.Domain.Enums;

namespace EventFlow.Operations.Application.Contracts;

public sealed record CheckInRequest(
    string QrCode,
    Guid? SectionId,
    Guid? SessionId,
    AttendanceMethod Method = AttendanceMethod.Qr);
