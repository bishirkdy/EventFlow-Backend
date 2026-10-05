using EventFlow.Event.Domain.Enums;
using MediatR;

namespace EventFlow.Event.Application.Features.Feedback.Queries.GetFeedbackResults;

public sealed record GetFeedbackResultsQuery(
    Guid EventId,
    Guid RequestedBy) : IRequest<GetFeedbackResultsResponse>;
