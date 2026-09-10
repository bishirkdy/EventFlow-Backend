

using AutoMapper;
using EventFlow.Event.Application.Features.PageSection.Queries.GetPageSections;
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Common.Mappings
{
    public sealed class PageSectionProfle : Profile
    {
        public PageSectionProfle()
        {
            CreateMap<PageSection, GetPageSectionsResponse>();
        }
    }
}
