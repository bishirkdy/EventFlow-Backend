using AutoMapper;
using EventFlow.Identity.Application.DTOs.Authentication;
using EventFlow.Identity.Domain.Entities;

namespace EventFlow.Identity.Application.Mapping
{
    public sealed class IdentityMappingProfile : Profile
    {
        public IdentityMappingProfile()
        {
            CreateMap<User, RegisterUserResponse>();
        }
    }
}
