using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Abstractions.Services;
using EventFlow.Identity.Application.DTOs.Authentication;
using EventFlow.Security.Configuration;
using EventFlow.Identity.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace EventFlow.Identity.Application.Features.Commands.RefreshAccessToken;

public sealed class RefreshAccessTokenCommandHandler(
    IRefreshTokenRepository refreshTokens,
    IUserRepository users,
    IJwtService jwtService,
    IRefreshTokenGenerator refreshTokenGenerator,
    IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<RefreshAccessTokenCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(
        RefreshAccessTokenCommand request,
        CancellationToken cancellationToken)
    {
        var current = await refreshTokens.GetByTokenAsync(
            request.RefreshToken,
            cancellationToken);

        if (current is null || !current.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid or inactive refresh token.");
        }

        var user = await users.GetByIdAsync(current.UserId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("The associated account is unavailable.");
        }

        current.Revoke();

        var accessToken = jwtService.GenerateAccessToken(user);
        var options = jwtOptions.Value;
        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(options.AccessTokenExpirationMinutes);
        var newRefreshTokenValue = refreshTokenGenerator.Generate();
        var newRefreshToken = new RefreshToken(
            user.Id,
            newRefreshTokenValue,
            DateTime.UtcNow.AddDays(options.RefreshTokenExpirationDays));

        await refreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await refreshTokens.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            user.Id,
            accessToken,
            newRefreshTokenValue,
            accessTokenExpiresAt);
    }
}
