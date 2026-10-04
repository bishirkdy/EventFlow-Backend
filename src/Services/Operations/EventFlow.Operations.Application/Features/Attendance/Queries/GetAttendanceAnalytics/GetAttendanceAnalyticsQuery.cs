using EventFlow.Contracts.Common;
using MediatR;

namespace EventFlow.Operations.Application.Features.Attendance.Queries.GetAttendanceAnalytics;

public sealed record GetAttendanceAnalyticsQuery(
    Guid EventId,
    int Days = 30)
    : IRequest<ApiResponse<GetAttendanceAnalyticsResponse>>;
