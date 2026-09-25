using AutoMapper;
using EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakersByEvent;
using EventFlow.Event.Domain.Entities;
namespace EventFlow.Event.Application.Common.Mappings;
public sealed class SpeakerMappingProfile : Profile
{
    public SpeakerMappingProfile() => CreateMap<Speaker, GetSpeakersByEventResponse>();
}
