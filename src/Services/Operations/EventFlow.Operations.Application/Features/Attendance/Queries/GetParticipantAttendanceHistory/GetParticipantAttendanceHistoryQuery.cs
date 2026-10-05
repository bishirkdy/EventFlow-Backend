using EventFlow.Operations.Application.Contracts;
using MediatR;

namespace EventFlow.Operations.Application.Features.Attendance.Queries.GetParticipantAttendanceHistory;

public sealed record GetParticipantAttendanceHistoryQuery(
    Guid EventId,
    Guid ParticipantUserId) : IRequest<AttendanceHistoryDto>;
