

using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Commands.LogoutUser
{
    public class LogoutUserCommandHandler
        : IRequestHandler<LogoutUserCommand>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LogoutUserCommandHandler(
            IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task Handle(
            LogoutUserCommand request,
            CancellationToken cancellationToken)
        {
            // Purpose: Find the refresh token in the database.
            var refreshToken =
                await _refreshTokenRepository.GetByTokenAsync(
                    request.RefreshToken,
                    cancellationToken);

            if (refreshToken is null)
            {
                throw new UnauthorizedException(
                    "Invalid refresh token.");
            }

            // Purpose: Prevent revoking an already inactive token.
            if (!refreshToken.IsActive)
            {
                throw new UnauthorizedException(
                    "Refresh token is no longer active.");
            }

            // Purpose: Revoke the refresh token.
            refreshToken.Revoke();

            // Purpose: Save the revoked state.
            await _refreshTokenRepository.SaveChangesAsync(
                cancellationToken);
        }
    }
}
