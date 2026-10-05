using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Application.Features.Tickets.Commands.RevokeTicket;
using EventFlow.Registration.Application.Features.Tickets.Queries.GetTicket;
using EventFlow.Registration.Application.Features.Tickets.Queries.VerifyQr;
using EventFlow.Registration.Application.Features.Tickets.Queries.VerifyQrInternal;
using EventFlow.Security.Authorization;
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
        var dto = await mediator.Send(
            new GetTicketQuery(eventId,registrationId), cancellationToken);

        return Ok(ApiResponse<TicketDto>.Success(dto));
    }

    [InternalServiceOnly]
    [HttpGet("verify-internal")]
    public async Task<IActionResult> VerifyInternal(Guid eventId,string qrCode,CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new VerifyQrInternalQuery(eventId,qrCode),cancellationToken);

        return Ok(ApiResponse<TicketDto>.Success(dto, "Ticket is valid."));
    }

    [Authorize]
    [HttpGet("verify")]
    public async Task<IActionResult> Verify(
        Guid eventId,
        string qrCode,
        CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(
            new VerifyQrQuery(
                eventId,
                qrCode),
            cancellationToken);

        return Ok(ApiResponse<TicketDto>.Success(dto, "Ticket is valid."));
    }

    [Authorize]
    [HttpPost("{ticketId:guid}/revoke")]
    public async Task<IActionResult> Revoke(
        Guid eventId,
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(
            new RevokeTicketCommand(
                eventId,
                ticketId),
            cancellationToken);

        return Ok(ApiResponse<TicketDto>.Success(dto, "Ticket revoked."));
    }
}
