using EventFlow.Contracts.Common;
using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.Notifications;

public sealed class NotificationService(IOperationsDbContext db, IEventAuthorizationClient authorization)
{
    public async Task<ApiResponse<NotificationDto>> QueueAsync(Guid eventId, Guid actorUserId, QueueNotificationRequest request, CancellationToken ct)
    {
        if (!await authorization.HasPermissionAsync(actorUserId,eventId,"event.update",ct)) return ApiResponse<NotificationDto>.Fail(["You do not have permission to send event notifications."]);
        if (string.IsNullOrWhiteSpace(request.RecipientEmail) || string.IsNullOrWhiteSpace(request.Subject)) return ApiResponse<NotificationDto>.Fail(["Recipient email and subject are required."]);
        var entity = new Notification { EventId=eventId, UserId=request.UserId, RecipientEmail=request.RecipientEmail.Trim(), Subject=request.Subject.Trim(), Body=request.Body, ScheduledAtUtc=request.ScheduledAtUtc ?? DateTime.UtcNow };
        db.Notifications.Add(entity); await db.SaveChangesAsync(ct);
        return ApiResponse<NotificationDto>.Success(ToDto(entity),"Notification queued successfully.");
    }
    public async Task<ApiResponse<IReadOnlyList<NotificationDto>>> ListAsync(Guid eventId, Guid actorUserId, CancellationToken ct)
    {
        if (!await authorization.HasPermissionAsync(actorUserId,eventId,"event.view",ct)) return ApiResponse<IReadOnlyList<NotificationDto>>.Fail(["You do not have permission to view notifications."]);
        var list=await db.Notifications.Where(x=>x.EventId==eventId).OrderByDescending(x=>x.CreatedAtUtc).Take(100).ToListAsync(ct);
        return ApiResponse<IReadOnlyList<NotificationDto>>.Success(list.Select(ToDto).ToList());
    }
    private static NotificationDto ToDto(Notification x)=>new(x.Id,x.EventId,x.UserId,x.RecipientEmail,x.Subject,x.Status,x.AttemptCount,x.ScheduledAtUtc,x.SentAtUtc,x.Error);
}
