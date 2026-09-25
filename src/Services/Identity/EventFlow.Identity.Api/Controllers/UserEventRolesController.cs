using EventFlow.Contracts.Common;
using EventFlow.Identity.Api.Contracts.UserEventRoles;
using EventFlow.Identity.Application.Features.Commands.AssignUserRole;
using EventFlow.Identity.Application.Features.Commands.RemoveUserRole;
using EventFlow.Identity.Application.Features.Queries.GetUserEventRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/events/{eventId}/users/{userId}/roles")]
public sealed class UserEventRolesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AssignRole(Guid eventId,Guid userId,[FromBody] AssignUserRoleRequest request,CancellationToken cancellationToken)
    {
        var roleId = await sender.Send(new AssignUserRoleCommand(userId,eventId,request.RoleId),cancellationToken);

        return Ok(ApiResponse<Guid>.Success(roleId,"User role assigned successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles(Guid eventId,Guid userId,CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUserEventRolesQuery(userId, eventId),cancellationToken);

        return Ok(ApiResponse<object?>.Success(result,"User roles retrieved successfully."));
    }

    [HttpDelete("{roleId:guid}")]
    public async Task<IActionResult> RemoveRole(Guid eventId,Guid userId,Guid roleId,CancellationToken cancellationToken)
    {
        await sender.Send(
            new RemoveUserRoleCommand(userId,eventId,roleId),cancellationToken);

        return Ok(ApiResponse<object?>.Success(null,"User role removed successfully."));
    }
}