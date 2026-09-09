

using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetCompleteEventPageBySlug
{
    public sealed record GetCompleteEventPageBySlugQuery(Guid EventId,string Slug): IRequest<GetCompleteEventPageBySlugResponse>;
}
