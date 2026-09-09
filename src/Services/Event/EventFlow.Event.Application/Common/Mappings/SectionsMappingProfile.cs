
using AutoMapper;
using EventFlow.Event.Application.Features.Sections.Queries.GetSectionById;
using EventFlow.Event.Application.Features.Sections.Queries.GetSectionsByEvent;
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Common.Mappings
{
    // Section Mapping
    public sealed class SectionMappingProfile : Profile
    {
        public SectionMappingProfile()
        {
            CreateMap<Section, GetSectionsByEventResponse>();
            CreateMap<Section, GetSectionByIdResponse>();
        }
    }
}
