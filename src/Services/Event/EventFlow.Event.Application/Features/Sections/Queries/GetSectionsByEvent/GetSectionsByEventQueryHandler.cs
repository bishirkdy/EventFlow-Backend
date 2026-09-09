using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;


namespace EventFlow.Event.Application.Features.Sections.Queries.GetSectionsByEvent
{
    public sealed class GetSectionsByEventQueryHandler(ISectionRepository sectionRepository, IMapper mapper)
        : IRequestHandler<GetSectionsByEventQuery, IReadOnlyList<GetSectionsByEventResponse>>
    {
        public async Task<IReadOnlyList<GetSectionsByEventResponse>> Handle(GetSectionsByEventQuery request, CancellationToken cancellationToken)
        {
            // Get sections
            var sections = await sectionRepository.GetByEventIdAsync(request.EventId,cancellationToken);

            // Map entities to responses
            return mapper.Map<IReadOnlyList<GetSectionsByEventResponse>>(sections);
        }
    }
}
