using EventFlow.Identity.Application.Commands.AssignUserRole;
using EventFlow.Identity.Application.Queries.GetUserEventRoles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Identity.Api.Controllers
{
    [ApiController]
    [Route("api/events/{eventId}/users/{userId}/roles")]
    public class UserEventRolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserEventRolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(
            Guid eventId,
            Guid userId,
            [FromBody] AssignUserRoleRequest request,
            CancellationToken cancellationToken)
        {
            // Purpose: Assign a role to a user for this event.
            var roleId = await _mediator.Send(
                new AssignUserRoleCommand(
                    userId,
                    eventId,
                    request.RoleId),
                cancellationToken);

            return Ok(new
            {
                Id = roleId
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles(
    Guid eventId,
    Guid userId,
    CancellationToken cancellationToken)
        {
            // Purpose: Get the user's roles for this event.
            var result = await _mediator.Send(
                new GetUserEventRolesQuery(
                    userId,
                    eventId),
                cancellationToken);

            return Ok(result);
        }
    }

    public class AssignUserRoleRequest
    {
        public Guid RoleId { get; set; }
    }


}
