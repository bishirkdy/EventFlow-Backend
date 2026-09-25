using AutoMapper;
using EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorsByEvent;
using EventFlow.Event.Domain.Entities;
namespace EventFlow.Event.Application.Common.Mappings;
public sealed class SponsorMappingProfile : Profile
{
    public SponsorMappingProfile() => CreateMap<Sponsor, GetSponsorsByEventResponse>();
}
