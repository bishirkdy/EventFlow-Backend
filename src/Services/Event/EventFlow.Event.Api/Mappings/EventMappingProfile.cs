using AutoMapper;
using EventFlow.Event.Api.Requests.Events;
using EventFlow.Event.Api.Requests.EventSettings;
using EventFlow.Event.Api.Requests.Sections;
using EventFlow.Event.Api.Requests.Sessions;
using EventFlow.Event.Api.Responses.Events;
using EventFlow.Event.Application.Features.Events.Commands.CreateEvent;
using EventFlow.Event.Application.Features.Events.Commands.UpdateEvent;
using EventFlow.Event.Application.Features.Events.Queries.GetEventById;
using EventFlow.Event.Application.Features.EventSettings.Commands.UpdateEventSettings;
using EventFlow.Event.Application.Features.Sections.Commands.CreateSection;
using EventFlow.Event.Application.Features.Sections.Commands.UpdateSection;
using EventFlow.Event.Application.Features.Sessions.Commands.CreateSession;
using EventFlow.Event.Application.Features.Sessions.Commands.UpdateSession;

namespace EventFlow.Event.Api.Mappings
{
    public sealed class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            //Event
            CreateMap<GetEventByIdResponse, GetEventResponse>();
            CreateMap<CreateEventRequest, CreateEventCommand>();
            CreateMap<UpdateEventRequest, UpdateEventCommand>();
           
            //Event settings
            CreateMap<UpdateEventSettingsRequest, UpdateEventSettingsCommand>();
            
            //Section
            CreateMap<CreateSectionRequest, CreateSectionCommand>();
            CreateMap<UpdateSectionRequest, UpdateSectionCommand>();

            //Session
            CreateMap<CreateSessionRequest, CreateSessionCommand>();
            CreateMap<UpdateSessionRequest, UpdateSessionCommand>();
        }
    }
}
