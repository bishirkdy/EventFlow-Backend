
using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Features.Registrations.Commands.ApproveRegistration;
using EventFlow.Registration.Application.Features.Registrations.Commands.CancelRegistration;
using EventFlow.Registration.Application.Features.Registrations.Commands.CreateRegistration;
using EventFlow.Registration.Application.Features.Registrations.Commands.PromoteWaitlistedRegistration;
using EventFlow.Registration.Application.Features.Registrations.Commands.RejectRegistration;
using EventFlow.Registration.Application.Features.Registrations.Commands.UpdateRegistration;
using EventFlow.Registration.Application.Features.Registrations.Commands.WaitlistRegistration;
using EventFlow.Registration.Application.Features.Registrations.Queries.GetEventRegistrations;
using EventFlow.Registration.Application.Features.Registrations.Queries.GetMyRegistrations;
using EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationById;
using EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationStats;
using EventFlow.Registration.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Registration.Api.Controllers;

//Controller for registrations
[ApiController]
[Route("api/v1/events/{eventId:guid}/registrations")]
public sealed class RegistrationsController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Guid eventId, CreateRegistrationRequest request, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(new CreateRegistrationCommand(eventId,request), cancellationToken);

        return Ok(ApiResponse<RegistrationDto>.Success(
            dto,dto.Status == RegistrationStatus.Waitlisted
                ? "Registration submitted and added to the waitlist."
                : "Registration submitted successfully."));
    }

    //for update registration by event
    [Authorize]
    [HttpPut("{registrationId:guid}")]
    public async Task<IActionResult> Update(Guid eventId,Guid registrationId, UpdateRegistrationRequest request, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new UpdateRegistrationCommand(eventId,registrationId,request), cancellationToken);

        return Ok(ApiResponse<RegistrationDto>.Success(
            dto,
            "Registration updated successfully."));
    }

    // Participant view own registration
    [Authorize]
    [HttpGet("{registrationId:guid}")]
    public async Task<IActionResult> GetMyRegistration(
        Guid eventId,
        Guid registrationId,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new GetRegistrationByIdQuery(
                eventId,
                registrationId,
                OrganizerView: false),
            cancellationToken);

        return Ok(ApiResponse<RegistrationDto>.Success(dto));
    }

    // Organizer view a participant's registration
    [Authorize]
    [HttpGet("organizer/{registrationId:guid}")]
    public async Task<IActionResult> GetForOrganizer(
        Guid eventId,
        Guid registrationId,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new GetRegistrationByIdQuery(
                eventId,
                registrationId,
                OrganizerView: true),
            cancellationToken);

        return Ok(ApiResponse<RegistrationDto>.Success(dto));
    }

    [Authorize]
    [HttpGet("{registrationId:guid}/manage")]
    public async Task<IActionResult> Manage(Guid eventId,Guid registrationId,CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new GetRegistrationByIdQuery(eventId,registrationId,true),cancellationToken);

        return Ok(ApiResponse<RegistrationDto>.Success(dto));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Mine(Guid eventId, CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new GetMyRegistrationsQuery(eventId), cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<RegistrationDto>>.Success(dto));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List(
        Guid eventId,RegistrationStatus? status,string? search,int page = 1,int pageSize = 20,CancellationToken cancellationToken = default)
    {
        var dto = await sender.Send(
            new GetEventRegistrationsQuery(eventId,status,search,page,pageSize),cancellationToken);

        return Ok(ApiResponse<PaginatedResponse<RegistrationDto>>.Success(dto));
    }

    [Authorize]
    [HttpGet("stats")]
    public async Task<IActionResult> Stats(Guid eventId,CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new GetRegistrationStatsQuery(eventId), cancellationToken);

        return Ok(ApiResponse<RegistrationStatsDto>.Success(dto));
    }

    [Authorize]
    [HttpPost("{registrationId:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid eventId,
        Guid registrationId,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new ApproveRegistrationCommand(
                eventId,
                registrationId),
            cancellationToken);

        return Ok(ApiResponse<RegistrationDto>.Success(
            dto,
            "Registration approved and ticket issued."));
    }

    [Authorize]
    [HttpPost("{registrationId:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid eventId,
        Guid registrationId,
        RejectRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new RejectRegistrationCommand(
                eventId,
                registrationId,
                request),
            cancellationToken);

        return Ok(ApiResponse<RegistrationDto>.Success(
            dto,
            "Registration rejected."));
    }

    [Authorize]
    [HttpPost("{registrationId:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid eventId,
        Guid registrationId,
        CancelRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new CancelRegistrationCommand(
                eventId,
                registrationId,
                request),
            cancellationToken);

        return Ok(ApiResponse<RegistrationDto>.Success(
            dto,
            "Registration cancelled."));
    }

    [Authorize]
    [HttpPost("{registrationId:guid}/waitlist")]
    public async Task<IActionResult> Waitlist(
        Guid eventId,
        Guid registrationId,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new WaitlistRegistrationCommand(
                eventId,
                registrationId),
            cancellationToken);

        return Ok(ApiResponse<RegistrationDto>.Success(
            dto,
            "Registration moved to waitlist."));
    }

    [Authorize]
    [HttpPost("{registrationId:guid}/promote")]
    public async Task<IActionResult> Promote(
        Guid eventId,
        Guid registrationId,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new PromoteWaitlistedRegistrationCommand(
                eventId,
                registrationId),
            cancellationToken);

        return Ok(ApiResponse<RegistrationDto>.Success(
            dto,
            "Waitlisted registration promoted."));
    }
}
