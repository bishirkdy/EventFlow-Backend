using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/events/{eventId:guid}/team")]
public sealed class EventTeamController(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserEventRoleRepository userEventRoleRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTeam(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(
                currentUser.UserId,
                eventId,
                "event.team.manage",
                cancellationToken))
        {
            return Forbid();
        }

        var assignments = await userEventRoleRepository.GetByEventAsync(
            eventId,
            cancellationToken);

        var members = assignments
            .GroupBy(x => new
            {
                x.UserId,
                x.User.Email,
                x.User.UserName,
                x.User.FirstName,
                x.User.LastName
            })
            .Select(group => new EventTeamMemberResponse(
                group.Key.UserId,
                group.Key.Email,
                group.Key.UserName,
                group.Key.FirstName,
                group.Key.LastName,
                group.Select(x => new EventTeamRoleResponse(x.RoleId, x.Role.Name)).ToList()))
            .ToList();

        return Ok(ApiResponse<IReadOnlyList<EventTeamMemberResponse>>.Success(
            members,
            "Event team retrieved successfully."));
    }

    [HttpPost("organizers")]
    public async Task<IActionResult> AssignOrganizer(
        Guid eventId,
        [FromBody] AssignOrganizerRequest request,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(
                currentUser.UserId,
                eventId,
                "event.team.manage",
                cancellationToken))
        {
            return Forbid();
        }

        var email = request.Email.Trim();
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(ApiResponse<object?>.Fail(
                ["Email is required."],
                "Invalid organizer request."));
        }

        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            return NotFound(ApiResponse<object?>.Fail(
                ["No user was found with that email address."],
                "User not found."));
        }

        var organizerRole = await roleRepository.GetByNameAsync("Organizer", cancellationToken);
        if (organizerRole is null)
        {
            return Conflict(ApiResponse<object?>.Fail(
                ["The Organizer role is not configured."],
                "Organizer role is unavailable."));
        }

        if (user.Id == currentUser.UserId)
        {
            return Conflict(ApiResponse<object?>.Fail(
                ["The event owner cannot be assigned as a separate organizer."],
                "Invalid organizer assignment."));
        }

        var alreadyAssigned = await userEventRoleRepository.ExistsAsync(
            user.Id,
            eventId,
            organizerRole.Id,
            cancellationToken);

        if (alreadyAssigned)
        {
            return Conflict(ApiResponse<object?>.Fail(
                ["This user is already an organizer for this event."],
                "Organizer already assigned."));
        }

        var assignment = new UserEventRole(user.Id, eventId, organizerRole.Id);
        await userEventRoleRepository.AddAsync(assignment, cancellationToken);
        await userEventRoleRepository.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<Guid>.Success(
            assignment.Id,
            "Organizer assigned successfully."));
    }

    [HttpDelete("organizers/{userId:guid}")]
    public async Task<IActionResult> RemoveOrganizer(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(
                currentUser.UserId,
                eventId,
                "event.team.manage",
                cancellationToken))
        {
            return Forbid();
        }

        var organizerRole = await roleRepository.GetByNameAsync("Organizer", cancellationToken);
        if (organizerRole is null)
        {
            return Conflict(ApiResponse<object?>.Fail(
                ["The Organizer role is not configured."],
                "Organizer role is unavailable."));
        }

        var assignments = await userEventRoleRepository.GetByUserAndEventAsync(
            userId,
            eventId,
            cancellationToken);

        var assignment = assignments.FirstOrDefault(x => x.RoleId == organizerRole.Id);
        if (assignment is null)
        {
            return NotFound(ApiResponse<object?>.Fail(
                ["Organizer assignment was not found."],
                "Organizer not found."));
        }

        await userEventRoleRepository.DeleteAsync(assignment);
        await userEventRoleRepository.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Organizer removed successfully."));
    }

    public sealed class AssignOrganizerRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public sealed record EventTeamMemberResponse(
        Guid UserId,
        string Email,
        string UserName,
        string FirstName,
        string LastName,
        IReadOnlyList<EventTeamRoleResponse> Roles);

    public sealed record EventTeamRoleResponse(Guid RoleId, string RoleName);
}
