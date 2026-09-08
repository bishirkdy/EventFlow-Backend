using AutoMapper;
using EventFlow.Event.Api.Requests.Events;
using EventFlow.Event.Api.Requests.EventSettings;
using EventFlow.Event.Api.Responses.Events;
using EventFlow.Event.Application.Features.Events.Commands.CreateEvent;
using EventFlow.Event.Application.Features.Events.Commands.UpdateEvent;
using EventFlow.Event.Application.Features.Events.Queries.GetEventById;
using EventFlow.Event.Application.Features.EventSettings.Commands.UpdateEventSettings;

namespace EventFlow.Event.Api.Mappings
{
    public sealed class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            CreateMap<GetEventByIdResponse, GetEventResponse>();
            CreateMap<CreateEventRequest, CreateEventCommand>();
            CreateMap<UpdateEventRequest, UpdateEventCommand>();
            CreateMap<UpdateEventSettingsRequest, UpdateEventSettingsCommand>();
        }
    }
}
