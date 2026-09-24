using AutoMapper;
using EventFlow.Event.Application.Features.Sessions.Queries.GetSessionById;
using EventFlow.Event.Application.Features.Sessions.Queries.GetSessionsbyEvent;
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Common.Mappings
{
    // Session Mapping
    public sealed class SessionMappingProfile : Profile
    {
        public SessionMappingProfile()
        {
            CreateMap<Session, GetSessionsByEventResponse>()
                .ForMember(d => d.StartTime, o => o.MapFrom(s => s.StartTimeUtc))
                .ForMember(d => d.EndTime, o => o.MapFrom(s => s.EndTimeUtc));
            CreateMap<Session, GetSessionByIdResponse>()
                .ForMember(d => d.StartTime, o => o.MapFrom(s => s.StartTimeUtc))
                .ForMember(d => d.EndTime, o => o.MapFrom(s => s.EndTimeUtc));
        }
    }
}
