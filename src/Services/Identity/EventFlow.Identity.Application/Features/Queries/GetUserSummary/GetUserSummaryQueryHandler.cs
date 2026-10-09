using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.Users;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetUserSummary;

public sealed class GetUserSummaryQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserSummaryQuery, UserSummaryResponse>
{
    public async Task<UserSummaryResponse> Handle(
        GetUserSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User was not found.");
        }

        var displayName = $"{user.FirstName} {user.LastName}".Trim();

        if (string.IsNullOrWhiteSpace(displayName))
        {
            displayName = user.UserName;
        }

        return new UserSummaryResponse(
            user.Id,
            user.UserName,
            user.Email,
            user.FirstName,
            user.LastName,
            displayName);
    }
}
