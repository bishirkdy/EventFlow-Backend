using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Features.Registrations.Queries.GetParticipantCount;
using EventFlow.Registration.Application.Features.Registrations.Queries.GetRegisteredEventIds;
using EventFlow.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;

[ApiController]
[Route("api/v1/registrations/internal")]
public sealed class InternalRegistrationsController(ISender sender) : ControllerBase
{
    [InternalServiceOnly]
    [HttpGet("users/{userId:guid}/event-ids")]
    public async Task<IActionResult> GetRegisteredEventIds(Guid userId, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(new GetRegisteredEventIdsQuery(userId), cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<Guid>>.Success(dto, "Registered event ids retrieved successfully."));
    }

    [InternalServiceOnly]
    [HttpGet("participants/count")]
    public async Task<IActionResult> GetParticipantCount(CancellationToken cancellationToken)
    {
        var count = await sender.Send(new GetParticipantCountQuery(), cancellationToken);

        return Ok(ApiResponse<int>.Success(count, "Participant count retrieved successfully."));
    }
}
