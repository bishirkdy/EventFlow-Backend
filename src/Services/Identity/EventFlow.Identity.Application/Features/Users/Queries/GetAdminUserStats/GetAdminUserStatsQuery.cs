using EventFlow.Identity.Application.DTOs.Admin;
using MediatR;

namespace EventFlow.Identity.Application.Features.Users.Queries.GetAdminUserStats;

public sealed record GetAdminUserStatsQuery : IRequest<AdminUserStatsResponse>;
