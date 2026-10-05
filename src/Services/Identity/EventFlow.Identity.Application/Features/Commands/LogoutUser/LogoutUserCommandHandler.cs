using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.LogoutUser;

public sealed class LogoutUserCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    : IRequestHandler<LogoutUserCommand>
{
    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return;
        }

        var refreshToken = await refreshTokenRepository.GetByTokenAsync(
            request.RefreshToken,
            cancellationToken);

        if (refreshToken is null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        if (!refreshToken.IsActive)
        {
            throw new UnauthorizedException("Refresh token is no longer active.");
        }

        refreshToken.Revoke();
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }
}
