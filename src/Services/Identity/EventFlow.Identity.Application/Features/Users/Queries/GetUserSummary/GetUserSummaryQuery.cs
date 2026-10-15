using EventFlow.Identity.Application.DTOs.Users;
using MediatR;

namespace EventFlow.Identity.Application.Features.Users.Queries.GetUserSummary;

public sealed record GetUserSummaryQuery(Guid UserId)
    : IRequest<UserSummaryResponse>;
