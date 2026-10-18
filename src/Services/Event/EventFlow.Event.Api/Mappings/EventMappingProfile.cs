using AutoMapper;
using EventFlow.Event.Api.Requests.EventPages;
using EventFlow.Event.Api.Requests.Events;
using EventFlow.Event.Api.Requests.PageSection;
using EventFlow.Event.Api.Requests.Sections;
using EventFlow.Event.Api.Responses.Events;
using EventFlow.Event.Application.Features.EventFeature.Queries.GetEventFeatures;
using EventFlow.Event.Application.Features.EventPages.Commands.CreateEventPage;
using EventFlow.Event.Application.Features.Events.Commands.CreateEvent;
using EventFlow.Event.Application.Features.Events.Commands.UpdateEvent;
using EventFlow.Event.Application.Features.Events.Queries.GetEventById;
using EventFlow.Event.Application.Features.PageSection.Commands.CreatePageSection;
using EventFlow.Event.Application.Features.PageSection.Commands.UpdatePageSection;
using EventFlow.Event.Application.Features.Sections.Commands.CreateSection;
using EventFlow.Event.Application.Features.Sections.Commands.UpdateSection;

using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Api.Mappings;

public sealed class EventMappingProfile : Profile
{
    public EventMappingProfile()
    {
        CreateMap<GetEventByIdResponse, GetEventResponse>()
            .ForMember(
                dest => dest.TimeZone,
                opt => opt.MapFrom(src => src.TimeZone))
            .ForMember(
                dest => dest.Images,
                opt => opt.MapFrom(src => src.Images));

        CreateMap<CreateEventResult, CreateEventResponse>();

        CreateMap<
            EventFlow.Event.Application.Features.Events.Common.GetEventResponse,
            EventFlow.Event.Api.Responses.Events.GetEventResponse>();

        CreateMap<UpdateEventRequest, UpdateEventCommand>();

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

        CreateMap<CreateSectionRequest, CreateSectionCommand>();
        CreateMap<UpdateSectionRequest, UpdateSectionCommand>();

        CreateMap<CreateEventPageRequest, CreateEventPageCommand>();

        CreateMap<CreatePageSectionRequest, CreatePageSectionCommand>();
        CreateMap<UpdatePageSectionRequest, UpdatePageSectionCommand>();

    }
}
