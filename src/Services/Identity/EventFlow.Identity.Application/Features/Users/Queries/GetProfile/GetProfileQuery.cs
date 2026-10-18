using EventFlow.Identity.Application.DTOs.Authentication;
using MediatR;

namespace EventFlow.Identity.Application.Features.Users.Queries.GetProfile;

public sealed record GetProfileQuery : IRequest<UserProfileResponse>;
