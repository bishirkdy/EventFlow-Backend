

using MediatR;

namespace EventFlow.Event.Application.Features.Sections.Queries.GetSectionsByEvent
{
    public sealed record GetSectionsByEventQuery(Guid EventId): IRequest<IReadOnlyList<GetSectionsByEventResponse>>;
}
