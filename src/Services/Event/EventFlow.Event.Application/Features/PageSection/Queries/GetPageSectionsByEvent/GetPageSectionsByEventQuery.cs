using EventFlow.Event.Application.Features.PageSection.Queries.GetPageSections;
using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Queries.GetPageSectionsByEvent;

public sealed record GetPageSectionsByEventQuery(
    Guid EventId,
    bool IncludeUnpublished = false)
    : IRequest<IReadOnlyList<GetPageSectionsResponse>>;
