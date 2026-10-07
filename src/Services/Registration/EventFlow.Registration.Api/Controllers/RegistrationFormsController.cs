using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Features.RegistrationForms.Commands.UpsertRegistrationForm;
using EventFlow.Registration.Application.Features.RegistrationForms.Queries.GetRegistrationForm;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;
//Controller for handling registration form
[ApiController]
[Route("api/v1/events/{eventId:guid}/registration-form")]
public sealed class RegistrationFormsController(ISender sender)
    : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get(Guid eventId, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(new GetRegistrationFormQuery(eventId), cancellationToken);

        return Ok(ApiResponse<RegistrationFormDto>.Success(dto));
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Put(Guid eventId, UpsertRegistrationFormRequest request, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(new UpsertRegistrationFormCommand(eventId,request), cancellationToken);
        return Ok(ApiResponse<RegistrationFormDto>.Success(dto, "Registration form saved."));
    }
}
