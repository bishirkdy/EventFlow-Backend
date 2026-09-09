using AutoMapper;
using EventFlow.Event.Application.Features.Venues.Queries.GetVenueById;
using EventFlow.Event.Application.Features.Venues.Queries.GetVenuesByEvent;
using EventFlow.Event.Domain.Entities;


namespace EventFlow.Event.Application.Common.Mappings
{
    public sealed class VenueMappingProfile : Profile
    {
        public VenueMappingProfile()
        {
            CreateMap<Venue, GetVenuesByEventResponse>();
            CreateMap<Venue, GetVenueByIdResponse>();
        }
    }
}
