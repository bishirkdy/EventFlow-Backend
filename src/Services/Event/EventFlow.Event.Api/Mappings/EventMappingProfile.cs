using AutoMapper;
using EventFlow.Event.Api.Requests.EventPages;
using EventFlow.Event.Api.Requests.Events;
using EventFlow.Event.Api.Requests.EventSettings;
using EventFlow.Event.Api.Requests.NavigationItem;
using EventFlow.Event.Api.Requests.NavigationItems;
using EventFlow.Event.Api.Requests.NavigationMenu;
using EventFlow.Event.Api.Requests.PageSection;
using EventFlow.Event.Api.Requests.Sections;
using EventFlow.Event.Api.Requests.Sessions;
using EventFlow.Event.Api.Requests.Venues;
using EventFlow.Event.Api.Responses.Events;
using EventFlow.Event.Application.Features.EventFeature.Queries.GetEventFeatures;
using EventFlow.Event.Application.Features.EventPages.Commands.CreateEventPage;
using EventFlow.Event.Application.Features.Events.Commands.CreateEvent;
using EventFlow.Event.Application.Features.Events.Commands.UpdateEvent;
using EventFlow.Event.Application.Features.Events.Queries.GetEventById;
using EventFlow.Event.Application.Features.EventSettings.Commands.UpdateEventSettings;
using EventFlow.Event.Application.Features.NavigationItem.Commands.CreateNavigationItem;
using EventFlow.Event.Application.Features.NavigationItem.Commands.UpdateNavigationItem;
using EventFlow.Event.Application.Features.NavigationMenu.Commands.CreateNavigationMenu;
using EventFlow.Event.Application.Features.NavigationMenu.Commands.UpdateNavigationMenu;
using EventFlow.Event.Application.Features.PageSection.Commands.CreatePageSection;
using EventFlow.Event.Application.Features.PageSection.Commands.UpdatePageSection;
using EventFlow.Event.Application.Features.Sections.Commands.CreateSection;
using EventFlow.Event.Application.Features.Sections.Commands.UpdateSection;
using EventFlow.Event.Application.Features.Sessions.Commands.CreateSession;
using EventFlow.Event.Application.Features.Sessions.Commands.UpdateSession;
using EventFlow.Event.Application.Features.Venues.Commands.CreateVenue;
using EventFlow.Event.Application.Features.Venues.Commands.UpdateVenue;
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Api.Mappings
{
    public sealed class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            //Event
            CreateMap<GetEventByIdResponse, GetEventResponse>()
                .ForMember(
                    dest => dest.TimeZone,
                    opt => opt.MapFrom(src => src.TimeZone))
                .ForMember(
                    dest => dest.Images,
                    opt => opt.MapFrom(src => src.Images));

            CreateMap<CreateEventResult, CreateEventResponse>();

            CreateMap<
                EventFlow.Event.Application.Features.Events.Queries.GetEvents.GetEventResponse,
                EventFlow.Event.Api.Responses.Events.GetEventResponse>();

            CreateMap<UpdateEventRequest, UpdateEventCommand>();

            //Event settings
            CreateMap<UpdateEventSettingsRequest, UpdateEventSettingsCommand>();

            //Event Features
            CreateMap<EventFeature, GetEventFeaturesResponse>()
                .ForMember(
                    dest => dest.FeatureCode,
                    opt => opt.MapFrom(src => src.Feature.Code))
                .ForMember(
                    dest => dest.FeatureName,
                    opt => opt.MapFrom(src => src.Feature.Name))
                .ForMember(
                    dest => dest.FeatureDescription,
                    opt => opt.MapFrom(src => src.Feature.Description));

            //Section
            CreateMap<CreateSectionRequest, CreateSectionCommand>();
            CreateMap<UpdateSectionRequest, UpdateSectionCommand>();

            //Event page
            CreateMap<CreateEventPageRequest, CreateEventPageCommand>();

            //Event page section
            CreateMap<CreatePageSectionRequest, CreatePageSectionCommand>();
            CreateMap<UpdatePageSectionRequest, UpdatePageSectionCommand>();

            //Navigation menu mapping
            CreateMap<CreateNavigationMenuRequest, CreateNavigationMenuCommand>();
            CreateMap<UpdateNavigationMenuRequest, UpdateNavigationMenuCommand>();

            //Navigation items mapping
            CreateMap<CreateNavigationItemRequest, CreateNavigationItemCommand>();
            CreateMap<UpdateNavigationItemRequest, UpdateNavigationItemCommand>();
        }
    }
}
