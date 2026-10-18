using AutoMapper;
using EventFlow.Event.Application.Features.Sessions.Queries.GetSessionById;
using EventFlow.Event.Application.Features.Sessions.Queries.GetSessionsbyEvent;
using EventFlow.Event.Domain.Entities;

namespace EventFlow.Event.Application.Common.Mappings;

public sealed class SessionMappingProfile : Profile
{
    public SessionMappingProfile()
    {
        CreateMap<Session, GetSessionsByEventResponse>()
            .ForCtorParam(nameof(GetSessionsByEventResponse.StartTime), o => o.MapFrom(s => s.StartTimeUtc))
            .ForCtorParam(nameof(GetSessionsByEventResponse.EndTime), o => o.MapFrom(s => s.EndTimeUtc));
        CreateMap<Session, GetSessionByIdResponse>()
            .ForCtorParam(nameof(GetSessionByIdResponse.StartTime), o => o.MapFrom(s => s.StartTimeUtc))
            .ForCtorParam(nameof(GetSessionByIdResponse.EndTime), o => o.MapFrom(s => s.EndTimeUtc));
    }
}
