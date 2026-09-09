using MediatR;


namespace EventFlow.Event.Application.Features.Sections.Queries.GetSectionById
{
    public sealed record GetSectionByIdQuery(Guid Id)
        : IRequest<GetSectionByIdResponse?>;
}
