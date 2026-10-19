using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Features.RegistrationForms.Commands.UpsertRegistrationForm;
using EventFlow.Registration.Application.Features.RegistrationForms.Queries.GetRegistrationForm;
using EventFlow.Registration.Domain.Enums;
using EventFlow.Security.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:guid}/registration-form")]
public sealed class RegistrationFormsController(
    ISender sender,
    IEventRegistrationAccessService eventAccess,
    ICurrentUserService currentUser)
    : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get(
        Guid eventId,
        [FromQuery] RegistrationScope scopeType = RegistrationScope.Event,
        [FromQuery] Guid? scopeId = null,
        CancellationToken cancellationToken = default)
    {
        // The public registration form is only available once the event is
        // published; event members may still open it while preparing.
        if (!await eventAccess.IsEventPublishedAsync(eventId, cancellationToken))
        {
            var canPreview = currentUser.IsAuthenticated &&
                await eventAccess.CanPreviewEventAsync(eventId, currentUser.UserId, cancellationToken);

            if (!canPreview)
            {
                return NotFound(ApiResponse<RegistrationFormDto>.Fail(
                    ["Registration is not open for this event."],
                    "Registration is not open for this event.",
                    System.Net.HttpStatusCode.NotFound));
            }
        }

        var dto = await sender.Send(new GetRegistrationFormQuery(eventId, scopeType, scopeId), cancellationToken);

        return Ok(ApiResponse<RegistrationFormDto>.Success(dto));
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Put(Guid eventId, UpsertRegistrationFormRequest request, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(new UpsertRegistrationFormCommand(eventId, request), cancellationToken);
        return Ok(ApiResponse<RegistrationFormDto>.Success(dto, "Registration form saved."));
    }
}
