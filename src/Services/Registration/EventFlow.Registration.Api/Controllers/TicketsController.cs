using EventFlow.Registration.Application.Features.Tickets.Commands.RevokeTicket;
using EventFlow.Registration.Application.Features.Tickets.Queries.GetTicket;
using EventFlow.Registration.Application.Features.Tickets.Queries.VerifyQr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:guid}/tickets")]
public sealed class TicketsController(IMediator mediator)
    : ControllerBase
{
    [Authorize]
    [HttpGet("registration/{registrationId:guid}")]
    public async Task<IActionResult> Get(Guid eventId, Guid registrationId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetTicketQuery(eventId,registrationId), cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : NotFound(result);
    }

    [Authorize]
    [HttpGet("verify")]
    public async Task<IActionResult> Verify(
        Guid eventId,
        string qrCode,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new VerifyQrQuery(
                eventId,
                qrCode),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [Authorize]
    [HttpPost("{ticketId:guid}/revoke")]
    public async Task<IActionResult> Revoke(
        Guid eventId,
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new RevokeTicketCommand(
                eventId,
                ticketId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }
}