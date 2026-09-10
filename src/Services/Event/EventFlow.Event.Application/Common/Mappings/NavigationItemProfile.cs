
using AutoMapper;
using EventFlow.Event.Application.Features.NavigationItem.Queries.GetNavigationItems;
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Common.Mappings
{
    public sealed class NavigationItemProfile : Profile
    {
        public NavigationItemProfile()
        {
            CreateMap<NavigationItem, GetNavigationItemsResponse>();
        }
    }
}
