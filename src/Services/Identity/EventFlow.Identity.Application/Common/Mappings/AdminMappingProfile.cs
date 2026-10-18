using AutoMapper;
using EventFlow.Identity.Application.DTOs.Admin;
using EventFlow.Identity.Domain.Entities;

namespace EventFlow.Identity.Application.Common.Mappings;

public sealed class AdminMappingProfile : Profile
{
    public AdminMappingProfile()
    {
        CreateMap<User, AdminUserListItemResponse>();
    }
}
