using EventFlow.Contracts.Common;
using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Entities;
using EventFlow.Operations.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.Attendance;

public sealed class AttendanceService(IOperationsDbContext db, IEventAuthorizationClient authorization, IRegistrationClient registrationClient, IEventScheduleClient scheduleClient)
{
    public async Task<ApiResponse<AttendanceDto>> CheckInQrAsync(Guid eventId, Guid actorUserId, CheckInRequest request, string? bearerToken, CancellationToken ct)
    {
        if (!await CanOperateAsync(eventId, actorUserId, request.SectionId, request.SessionId, ct))
            return ApiResponse<AttendanceDto>.Fail(["You are not assigned to this attendance scope."]);
        var verified = await registrationClient.VerifyTicketAsync(eventId, request.QrCode.Trim(), bearerToken, ct);
        if (verified is null || !verified.IsValid) return ApiResponse<AttendanceDto>.Fail([verified?.Message ?? "Ticket verification failed."]);

        var existing = await db.AttendanceRecords.SingleOrDefaultAsync(x => x.EventId == eventId && x.RegistrationId == verified.RegistrationId && x.SessionId == request.SessionId, ct);
        if (existing is not null) return ApiResponse<AttendanceDto>.Success(ToDto(existing), "Participant is already checked in.");

        var entity = new AttendanceRecord { EventId=eventId, RegistrationId=verified.RegistrationId, ParticipantId=verified.ParticipantId, ParticipantUserId=verified.ParticipantUserId, SectionId=request.SectionId, SessionId=request.SessionId, StaffUserId=actorUserId, Method=request.Method, CheckedInAtUtc=DateTime.UtcNow };
        db.AttendanceRecords.Add(entity); await db.SaveChangesAsync(ct);
        return ApiResponse<AttendanceDto>.Success(ToDto(entity), "Participant checked in successfully.");
    }

    public async Task<ApiResponse<AttendanceDto>> ManualCheckInAsync(Guid eventId, Guid actorUserId, ManualCheckInRequest request, CancellationToken ct)
    {
        if (!await CanOperateAsync(eventId, actorUserId, request.SectionId, request.SessionId, ct)) return ApiResponse<AttendanceDto>.Fail(["You are not assigned to this attendance scope."]);
        var entity = new AttendanceRecord { EventId=eventId, RegistrationId=request.RegistrationId, ParticipantId=Guid.Empty, ParticipantUserId=Guid.Empty, SectionId=request.SectionId, SessionId=request.SessionId, StaffUserId=actorUserId, Method=AttendanceMethod.Manual, CheckedInAtUtc=DateTime.UtcNow };
        db.AttendanceRecords.Add(entity); await db.SaveChangesAsync(ct);
        return ApiResponse<AttendanceDto>.Success(ToDto(entity), "Participant checked in manually.");
    }

    public async Task<ApiResponse<AttendanceDto>> CheckOutAsync(Guid eventId, Guid attendanceId, Guid actorUserId, CancellationToken ct)
    {
        var record = await db.AttendanceRecords.SingleOrDefaultAsync(x => x.Id == attendanceId && x.EventId == eventId, ct);
        if (record is null) return ApiResponse<AttendanceDto>.Fail(["Attendance record not found."]);
        if (!await CanOperateAsync(eventId, actorUserId, record.SectionId, record.SessionId, ct)) return ApiResponse<AttendanceDto>.Fail(["You are not assigned to this attendance scope."]);
        if (record.CheckedOutAtUtc is null) { record.CheckedOutAtUtc=DateTime.UtcNow; await db.SaveChangesAsync(ct); }
        return ApiResponse<AttendanceDto>.Success(ToDto(record), "Participant checked out successfully.");
    }

    public async Task<ApiResponse<AttendanceDashboardDto>> DashboardAsync(Guid eventId, Guid actorUserId, CancellationToken ct)
    {
        if (!await authorization.HasPermissionAsync(actorUserId,eventId,"event.view",ct)) return ApiResponse<AttendanceDashboardDto>.Fail(["You do not have permission to view attendance."]);
        var records = await db.AttendanceRecords.Where(x=>x.EventId==eventId).ToListAsync(ct);
        var distinct = records.Select(x=>x.RegistrationId).Distinct().Count();
        var checkedOut = records.Count(x=>x.CheckedOutAtUtc.HasValue);
        var inside = records.Count(x=>!x.CheckedOutAtUtc.HasValue);
        return ApiResponse<AttendanceDashboardDto>.Success(new AttendanceDashboardDto(distinct,distinct,checkedOut,inside,distinct==0?0:100d));
    }

    public async Task<ApiResponse<AttendanceHistoryDto>> ParticipantHistoryAsync(Guid eventId, Guid participantUserId, Guid actorUserId, CancellationToken ct)
    {
        if (actorUserId != participantUserId && !await authorization.HasPermissionAsync(actorUserId,eventId,"event.view",ct)) return ApiResponse<AttendanceHistoryDto>.Fail(["You do not have access to this attendance history."]);
        var records = await db.AttendanceRecords.Where(x=>x.EventId==eventId && x.ParticipantUserId==participantUserId).ToListAsync(ct);
        var totalSessions=await scheduleClient.GetSessionCountAsync(eventId,ct); var attendedSessions=records.Count(x=>x.SessionId.HasValue); var percentage=totalSessions==0?0:Math.Round(attendedSessions*100d/totalSessions,2); return ApiResponse<AttendanceHistoryDto>.Success(new AttendanceHistoryDto(eventId,records.Count(x=>x.SessionId==null),attendedSessions,totalSessions,percentage));
    }

    private async Task<bool> CanOperateAsync(Guid eventId, Guid userId, Guid? sectionId, Guid? sessionId, CancellationToken ct)
    {
        if (await authorization.HasPermissionAsync(userId,eventId,"event.team.manage",ct)) return true;
        return await db.AttendanceStaffAssignments.AnyAsync(x => x.EventId==eventId && x.UserId==userId && x.IsActive && (x.ScopeType==AttendanceScopeType.Event || (x.ScopeType==AttendanceScopeType.Section && x.ScopeId==sectionId) || (x.ScopeType==AttendanceScopeType.Session && x.ScopeId==sessionId)),ct);
    }
    private static AttendanceDto ToDto(AttendanceRecord x) => new(x.Id,x.EventId,x.RegistrationId,x.ParticipantId,x.ParticipantUserId,x.SectionId,x.SessionId,x.StaffUserId,x.Method,x.CheckedInAtUtc,x.CheckedOutAtUtc);
}
