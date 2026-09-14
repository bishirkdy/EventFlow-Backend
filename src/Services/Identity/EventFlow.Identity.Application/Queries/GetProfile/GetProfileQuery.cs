using EventFlow.Identity.Application.DTOs.Authentication;
using MediatR;

namespace EventFlow.Identity.Application.Queries.GetProfile
{
    public sealed record GetProfileQuery : IRequest<UserProfileResponse>;
}
