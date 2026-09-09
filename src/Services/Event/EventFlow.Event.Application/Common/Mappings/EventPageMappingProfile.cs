using AutoMapper;
using EventFlow.Event.Application.Features.EventPages.Queries.GetEventPagesByEvent;
using EventFlow.Event.Application.Features.EventSettings.Queries.GetEventSettings;
using EventFlow.Event.Domain.Entities;


namespace EventFlow.Event.Application.Common.Mappings
{
    public sealed class EventPageMappingProfile : Profile
    {
        public EventPageMappingProfile()
        {
            CreateMap<EventPage, GetEventPagesByEventResponse>();
        }
    }
}
