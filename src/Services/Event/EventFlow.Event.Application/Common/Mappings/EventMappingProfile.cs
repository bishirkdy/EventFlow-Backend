using AutoMapper;
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
            CreateMap<EventEntity, EventResponse>();
            CreateMap<EventEntity, GetEventByIdResponse>()
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            // Event entity > Get My Events response
            CreateMap<EventEntity, GetMyEventsResponse>()
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
