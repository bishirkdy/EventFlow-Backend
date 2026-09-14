

using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.Authentication;
using MediatR;

namespace EventFlow.Identity.Application.Queries.GetProfile
{
    public sealed class GetProfileQueryHandler(ICurrentUserService currentUserService, IUserRepository userRepository)
        : IRequestHandler<GetProfileQuery, UserProfileResponse>
    {
        public async Task<UserProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(currentUserService.UserId, cancellationToken);

            if (user is null)
                throw new UnauthorizedAccessException("User not found.");

            return new UserProfileResponse(
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName
            );
        }
    }
}
