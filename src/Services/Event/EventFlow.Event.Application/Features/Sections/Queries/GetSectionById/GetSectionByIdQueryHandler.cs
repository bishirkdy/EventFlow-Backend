

using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Sections.Queries.GetSectionById
{
    public sealed class GetSectionByIdQueryHandler(ISectionRepository sectionRepository,IMapper mapper)
        : IRequestHandler<GetSectionByIdQuery, GetSectionByIdResponse?>
    {
        public async Task<GetSectionByIdResponse?> Handle(GetSectionByIdQuery request,CancellationToken cancellationToken)
        {
            // Get section
            var section = await sectionRepository.GetByIdAsync(request.Id,cancellationToken);

            if (section is null)
                return null;

            // Map entity to response
            return mapper.Map<GetSectionByIdResponse>(section);
        }
    }
}
