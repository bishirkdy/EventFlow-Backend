using EventFlow.Contracts.Common;
using EventFlow.Identity.Application.DTOs.Admin;
using MediatR;

namespace EventFlow.Identity.Application.Features.Users.Queries.GetAdminUsers;

public sealed record GetAdminUsersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null
) : IRequest<PaginatedResponse<AdminUserListItemResponse>>;
