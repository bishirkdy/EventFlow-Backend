using AutoMapper;
using EventFlow.Event.Application.Features.Admin.Queries.GetAdminEvents;
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Common.Mappings;

public sealed class AdminMappingProfile : Profile
{
    public AdminMappingProfile()
    {
        CreateMap<EventEntity, AdminEventListItemResponse>()
            .ForMember(
                dest => dest.EventType,
                opt => opt.MapFrom(src => src.EventType.Name))
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
