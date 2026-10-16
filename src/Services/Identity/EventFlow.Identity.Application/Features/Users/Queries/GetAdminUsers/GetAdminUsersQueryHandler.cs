using AutoMapper;
using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.Admin;
using MediatR;

namespace EventFlow.Identity.Application.Features.Users.Queries.GetAdminUsers;

public sealed class GetAdminUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    : IRequestHandler<GetAdminUsersQuery, PaginatedResponse<AdminUserListItemResponse>>
{
    public async Task<PaginatedResponse<AdminUserListItemResponse>> Handle(
        GetAdminUsersQuery request,
        CancellationToken cancellationToken)
    {
        var result = await userRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            cancellationToken);

        return new PaginatedResponse<AdminUserListItemResponse>
        {
            Items = mapper.Map<List<AdminUserListItemResponse>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
