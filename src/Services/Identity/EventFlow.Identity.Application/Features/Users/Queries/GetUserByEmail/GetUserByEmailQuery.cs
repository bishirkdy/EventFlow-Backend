using EventFlow.Identity.Application.DTOs.Users;
using MediatR;

namespace EventFlow.Identity.Application.Features.Users.Queries.GetUserByEmail;

public sealed record GetUserByEmailQuery(string Email)
    : IRequest<UserSummaryResponse>;
