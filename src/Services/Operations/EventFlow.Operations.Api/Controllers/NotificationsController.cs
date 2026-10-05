using EventFlow.Contracts.Common;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Application.Features.Notifications.Commands.QueueNotification;
using EventFlow.Operations.Application.Features.Notifications.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Operations.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/operations/events/{eventId:guid}/notifications")]
public sealed class NotificationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<NotificationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetNotificationsQuery(eventId),
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<NotificationDto>>.Success(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Queue(
        Guid eventId,
        QueueNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await sender.Send(
            new QueueNotificationCommand(
                eventId,
                request.UserId,
                request.RecipientEmail,
                request.Subject,
                request.Body,
                request.ScheduledAtUtc),
            cancellationToken);

        return Ok(ApiResponse<NotificationDto>.Success(
            dto,
            "Notification queued successfully."));
    }
}
