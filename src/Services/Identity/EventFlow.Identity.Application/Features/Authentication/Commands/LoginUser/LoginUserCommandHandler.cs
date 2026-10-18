using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Abstractions.Services;
using EventFlow.Identity.Application.DTOs.Authentication;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Security.Configuration;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace EventFlow.Identity.Application.Features.Authentication.Commands.LoginUser;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly JwtOptions _jwtOptions;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IOptions<JwtOptions> jwtOptions,
        IRefreshTokenGenerator refreshTokenGenerator)

    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _jwtOptions = jwtOptions.Value;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task<LoginResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        // Use the same error for unknown email and password so the API does
        // not reveal whether an account exists.
        if (user is null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException("This account is inactive.");
        }

        var passwordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!passwordValid)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

        var refreshTokenValue = _refreshTokenGenerator.Generate();
        var refreshToken = new RefreshToken(user.Id, refreshTokenValue, DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays));

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        return new LoginResponse(user.Id, accessToken, refreshTokenValue, accessTokenExpiresAt);
    }
}
