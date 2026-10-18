using AutoMapper;
using EventFlow.Contracts.Common;
using EventFlow.Event.Api.Services;
using EventFlow.Event.Application.Features.EventFeature.Commands.DisableEventFeature;
using EventFlow.Event.Application.Features.EventFeature.Commands.EnableEventFeature;
using EventFlow.Event.Application.Features.EventFeature.Commands.ResetEventFeatures;
using EventFlow.Event.Application.Features.EventFeature.Queries.GetEventFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/events")]
[Authorize]
public sealed class EventFeatureController(ISender sender, IEventPreviewAccess previewAccess) : ControllerBase
{
    [HttpPost("{eventId:guid}/features/{featureId:guid}/enable")]
    public async Task<IActionResult> EnableFeature(Guid eventId, Guid featureId, CancellationToken cancellationToken)
    {
        await sender.Send(new EnableEventFeatureCommand(eventId, featureId), cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Feature enabled successfully."));
    }

    [HttpPost("{eventId:guid}/features/{featureId:guid}/disable")]
    public async Task<IActionResult> DisableFeature(Guid eventId, Guid featureId, CancellationToken cancellationToken)
    {
        await sender.Send(
            new DisableEventFeatureCommand(eventId, featureId),
            cancellationToken);

        return Ok(ApiResponse<object?>.Success(
            null,
            "Feature disabled successfully."));
    }

    [AllowAnonymous]
    [HttpGet("{eventId:guid}/features")]
    public async Task<IActionResult> GetEventFeatures(Guid eventId, CancellationToken cancellationToken)
    {
        if (!await previewAccess.CanReadAsync(eventId, cancellationToken))
        {
            return Ok(ApiResponse<IReadOnlyList<GetEventFeaturesResponse>>.Success(
                [],
                "Event features retrieved successfully."));
        }

        var features = await sender.Send(new GetEventFeaturesQuery(eventId), cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetEventFeaturesResponse>>.Success(
            features,
            "Event features retrieved successfully."));
    }

    [HttpPost("{eventId:guid}/features/reset")]
    public async Task<IActionResult> ResetEventFeatures(Guid eventId, CancellationToken cancellationToken)
    {
        var features = await sender.Send(
            new ResetEventFeaturesCommand(eventId), cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<GetEventFeaturesResponse>>.Success(
            features,
            "Event features reset successfully."));
    }
}
