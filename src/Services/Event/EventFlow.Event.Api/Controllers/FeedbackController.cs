using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Features.Feedback.Commands.SubmitFeedback;
using EventFlow.Event.Application.Features.Feedback.Queries.GetFeedbackResults;
using EventFlow.Event.Api.Requests.Feedback;
using EventFlow.Security.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:guid}/feedback")]
[Authorize]
public sealed class FeedbackController(ISender sender, ICurrentUserService currentUser) : ControllerBase
{
    // POST /api/v1/events/{eventId}/feedback - Submit participant feedback
    [HttpPost]
    public async Task<IActionResult> Submit(
        Guid eventId,
        SubmitFeedbackRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SubmitFeedbackCommand(
            eventId,
            currentUser.UserId,
            request.TargetType,
            request.TargetId,
            request.Rating,
            request.Comment);

        var response = await sender.Send(command, cancellationToken);

        return Ok(ApiResponse<SubmitFeedbackResponse>.Success(
            response,
            "Thank you for your feedback."));
    }

    // GET /api/v1/events/{eventId}/feedback/results - Organizer feedback results
    [HttpGet("results")]
    public async Task<IActionResult> Results(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var query = new GetFeedbackResultsQuery(eventId, currentUser.UserId);
        var response = await sender.Send(query, cancellationToken);

        return Ok(ApiResponse<GetFeedbackResultsResponse>.Success(
            response,
            "Feedback results retrieved successfully."));
    }
}
