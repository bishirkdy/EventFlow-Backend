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
            CreateMap<Session, GetSessionsByEventResponse>();
            CreateMap<Session, GetSessionByIdResponse>();
        }
    }
}
