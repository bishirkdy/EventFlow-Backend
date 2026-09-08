using AutoMapper;
using EventFlow.Event.Application.Features.EventSettings.Queries.GetEventSettings;
using EventFlow.Event.Domain.Entities;


namespace EventFlow.Event.Application.Common.Mappings
{
    // Event Settings Mapping
    public sealed class EventSettingsMappingProfile : Profile
    {
        public EventSettingsMappingProfile()
        {
            // Entity → Query Response
            CreateMap<EventSettings, GetEventSettingsResponse>();
        }
    }
}
