using EventFlow.Operations.Domain.Enums;

namespace EventFlow.Operations.Application.Contracts;

public sealed record AttendanceDto(
    Guid Id,
    Guid EventId,
    Guid RegistrationId,
    Guid ParticipantId,
    Guid ParticipantUserId,
    Guid? SectionId,
    Guid? SessionId,
    Guid StaffUserId,
    AttendanceMethod Method,
    DateTime CheckedInAtUtc,
    DateTime? CheckedOutAtUtc);
