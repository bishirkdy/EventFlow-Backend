using AutoMapper;
using EventFlow.Identity.Application.DTOs.Admin;
using EventFlow.Identity.Domain.Entities;

namespace EventFlow.Identity.Application.Common.Mappings
{
    public sealed class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            // Project the user entity into the admin list item.
            CreateMap<User, AdminUserListItemResponse>();
        }
    }
}
