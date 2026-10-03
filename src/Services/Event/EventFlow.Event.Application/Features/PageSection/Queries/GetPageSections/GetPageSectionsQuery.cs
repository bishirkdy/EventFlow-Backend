using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Queries.GetPageSections;

public sealed record GetPageSectionsQuery(
    Guid PageId,
    bool IncludeUnpublished = false)
    : IRequest<IReadOnlyList<GetPageSectionsResponse>>;
