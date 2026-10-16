using AutoMapper;
using AppItem = EventFlow.Event.Application.Features.Admin.Queries.GetAdminEvents.AdminEventListItemResponse;
using ApiItem = EventFlow.Event.Api.Responses.Admin.AdminEventListItemResponse;

namespace EventFlow.Event.Api.Mappings
{
    public sealed class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            // Map the application admin event item into the API response.
            CreateMap<AppItem, ApiItem>();
        }
    }
}
