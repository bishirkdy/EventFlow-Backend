using EventFlow.Operations.Domain.Enums;

namespace EventFlow.Operations.Application.Contracts;

public sealed record AssignAttendanceStaffRequest(
    Guid UserId,
    AttendanceScopeType ScopeType,
    Guid? ScopeId);

public sealed record CheckInRequest(
    string QrCode,
    Guid? SectionId,
    Guid? SessionId,
    AttendanceMethod Method = AttendanceMethod.Qr);

public sealed record ManualCheckInRequest(
    Guid RegistrationId,
    Guid? SectionId,
    Guid? SessionId);
