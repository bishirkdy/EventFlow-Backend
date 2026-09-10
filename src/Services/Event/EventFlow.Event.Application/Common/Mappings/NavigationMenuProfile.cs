
using AutoMapper;
using EventFlow.Event.Application.Features.NavigationMenu.Queries.GetNavigationMenuById;
using EventFlow.Event.Application.Features.NavigationMenu.Queries.GetNavigationMenusByEvent;
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Common.Mappings
{
    public sealed class NavigationMenuProfile : Profile
    {
        NavigationMenuProfile()
        {
            CreateMap<NavigationMenu, GetNavigationMenusByEventResponse>();
            CreateMap<NavigationMenu,GetNavigationMenuByIdResponse>();
        }
    }
}
