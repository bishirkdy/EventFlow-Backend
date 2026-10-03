using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetEventPagesByEvent;

public sealed record GetEventPagesByEventQuery(
    Guid EventId,
    bool IncludeUnpublished = false)
    : IRequest<IReadOnlyList<GetEventPagesByEventResponse>>;
