using EventFlow.Registration.Application.Features.RegistrationForms.Commands.UpsertRegistrationForm;
using EventFlow.Registration.Application.Features.RegistrationForms.Queries.GetRegistrationForm;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:guid}/registration-form")]
public sealed class RegistrationFormsController(IMediator mediator)
    : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetRegistrationFormQuery(eventId),
            cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Put(
        Guid eventId,
        UpsertRegistrationFormRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpsertRegistrationFormCommand(
                eventId,
                request),
            cancellationToken);

        return Ok(result);
    }
}