using EventFlow.Contracts.Common;
using EventFlow.Identity.Api.Contracts.Authorization;
using EventFlow.Identity.Application.Features.Commands.AssignEventRole;
using EventFlow.Identity.Application.Features.Commands.AssignOwnerRole;
using EventFlow.Identity.Application.Features.Commands.RemoveEventRole;
using EventFlow.Identity.Application.Features.Queries.CheckPermission;
using EventFlow.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[ApiController]
[InternalServiceOnly]
[Route("api/authorization/v1")]
public sealed class AuthorizationController(ISender sender) : ControllerBase
{
    [HttpPost("assign-owner")]
    public async Task<IActionResult> AssignOwner(
        AssignOwnerRequest request,
        CancellationToken cancellationToken)
    {
        var roleId = await sender.Send(
            new AssignOwnerRoleCommand(request.UserId, request.EventId),
            cancellationToken);

        return Ok(ApiResponse<Guid>.Success(
            roleId,
            "Event owner assigned successfully."));
    }

    [HttpPost("check-permission")]
    public async Task<IActionResult> CheckPermission(CheckPermissionRequest request,CancellationToken cancellationToken)
    {
        var hasPermission = await sender.Send(new CheckPermissionQuery(request.UserId, request.EventId, request.Permission),cancellationToken);

        return Ok(ApiResponse<bool>.Success(
            hasPermission,
            "Permission check completed successfully."));
    }

    [HttpPost("assign-event-role")]
    public async Task<IActionResult> AssignEventRole(
        AssignEventRoleRequest request,
        CancellationToken cancellationToken)
    {
        var userEventRoleId = await sender.Send(
            new AssignEventRoleCommand(
                request.UserId,
                request.EventId,
                request.RoleName),
            cancellationToken);

        return Ok(ApiResponse<Guid>.Success(
            userEventRoleId,
            "Event role assigned successfully."));
    }

    [HttpPost("remove-event-role")]
    public async Task<IActionResult> RemoveEventRole(RemoveEventRoleRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveEventRoleCommand(request.UserId,request.EventId,request.RoleName), cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "Event role removed successfully."));
    }
}
