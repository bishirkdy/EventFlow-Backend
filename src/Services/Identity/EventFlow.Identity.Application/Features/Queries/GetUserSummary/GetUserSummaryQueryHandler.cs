using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.Users;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetUserSummary;

public sealed class GetUserSummaryQueryHandler(IUserRepository userRepository): IRequestHandler<GetUserSummaryQuery, UserSummaryResponse?>
{
    public async Task<UserSummaryResponse?> Handle(GetUserSummaryQuery request,CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId,cancellationToken);

        if (user is null)
            return null;

        var displayName = $"{user.FirstName} {user.LastName}".Trim();

        if (string.IsNullOrWhiteSpace(displayName))
            displayName = user.UserName;

        return new UserSummaryResponse(
            user.Id,
            user.UserName,
            user.FirstName,
            user.LastName,
            displayName);
    }
}