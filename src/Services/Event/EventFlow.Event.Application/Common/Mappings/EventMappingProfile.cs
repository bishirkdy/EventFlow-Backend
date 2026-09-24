using AutoMapper;
using EventFlow.Event.Application.Features.Events.Common;
using EventFlow.Event.Application.Features.Events.Queries.GetEventById;
using EventFlow.Event.Application.Features.Events.Queries.GetEvents;
using EventFlow.Event.Application.Features.Events.Queries.GetMyEvents;
using EventFlow.Event.Domain.Entities;


namespace EventFlow.Event.Application.Common.Mappings
{
    public sealed class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            CreateMap<EventImage, EventImageResponse>();

            CreateMap<EventEntity, GetEventResponse>()
                .ForMember(
                    dest => dest.EventType,
                    opt => opt.MapFrom(src => src.EventType.Name))
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(
                    dest => dest.Images,
                    opt => opt.MapFrom(src =>
                        src.Images.OrderBy(image => image.DisplayOrder)));
            CreateMap<EventEntity, GetEventByIdResponse>()
                .ForMember(
                    dest => dest.EventType,
                    opt => opt.MapFrom(src => src.EventType.Name))
                .ForMember(
                    dest => dest.TimeZone,
                    opt => opt.MapFrom(src => src.TimeZone))
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            // Event entity > Get My Events response
            CreateMap<EventEntity, GetMyEventsResponse>()
                .ForMember(
                    dest => dest.EventType,
                    opt => opt.MapFrom(src => src.EventType.Name))
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(
                    dest => dest.Images,
                    opt => opt.MapFrom(src =>
                        src.Images.OrderBy(image => image.DisplayOrder)));
                    }
    }
}
