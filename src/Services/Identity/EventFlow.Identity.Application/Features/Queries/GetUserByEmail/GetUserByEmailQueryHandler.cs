using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.Users;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetUserByEmail;

public sealed class GetUserByEmailQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByEmailQuery, UserSummaryResponse>
{
    public async Task<UserSummaryResponse> Handle(
        GetUserByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(
            request.Email.Trim(),
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User was not found.");
        }

        return new UserSummaryResponse(
            user.Id,
            user.UserName,
            user.FirstName,
            user.LastName,
            $"{user.FirstName} {user.LastName}".Trim());
    }
}
