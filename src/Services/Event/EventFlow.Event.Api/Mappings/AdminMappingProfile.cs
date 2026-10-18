using AutoMapper;
using ApiItem = EventFlow.Event.Api.Responses.Admin.AdminEventListItemResponse;
using AppItem = EventFlow.Event.Application.Features.Admin.Queries.GetAdminEvents.AdminEventListItemResponse;

namespace EventFlow.Event.Api.Mappings;

public sealed class AdminMappingProfile : Profile
{
    public AdminMappingProfile()
    {
        CreateMap<AppItem, ApiItem>();
    }
}
