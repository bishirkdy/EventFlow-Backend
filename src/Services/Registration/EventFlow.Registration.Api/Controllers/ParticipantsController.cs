using EventFlow.Registration.Application.Features.Participants.Queries.GetEventParticipants;
using EventFlow.Registration.Application.Features.Participants.Queries.GetParticipantById;
using EventFlow.Registration.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/events/{eventId:guid}/participants")]
public sealed class ParticipantsController(IMediator mediator)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        Guid eventId,
        ParticipantStatus? status,
        string? search,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetEventParticipantsQuery(
                eventId,
                status,
                search,
                page,
                pageSize),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{participantId:guid}")]
    public async Task<IActionResult> Get(
        Guid eventId,
        Guid participantId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetParticipantByIdQuery(
                eventId,
                participantId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : NotFound(result);
    }
}