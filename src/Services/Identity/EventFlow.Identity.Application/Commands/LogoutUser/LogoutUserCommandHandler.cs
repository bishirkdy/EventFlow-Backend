

using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Commands.LogoutUser
{
    //Handle logout using refresh token
    public sealed class LogoutUserCommandHandler(IRefreshTokenRepository refreshTokenRepository): IRequestHandler<LogoutUserCommand>
    {
        public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            //  Find the refresh token in the database.
            var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

            if (refreshToken is null)
            {
                throw new UnauthorizedException("Invalid refresh token.");
            }

            //  Prevent revoking an already inactive token.
            if (!refreshToken.IsActive)
            {
                throw new UnauthorizedException("Refresh token is no longer active.");
            }

            // Revoke the refresh token.
            refreshToken.Revoke();

            // Save the revoked state.
            await refreshTokenRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
