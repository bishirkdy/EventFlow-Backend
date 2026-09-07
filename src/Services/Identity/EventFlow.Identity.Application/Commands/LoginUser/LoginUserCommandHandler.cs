using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Abstractions.Services;
using EventFlow.Identity.Application.Commands.Login;
using EventFlow.Identity.Application.Configuration;
using EventFlow.Identity.Application.DTOs.Authentication;
using EventFlow.Identity.Application.Exceptions;
using EventFlow.Identity.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;


namespace EventFlow.Identity.Application.Commands.LoginUser
{
    //Handler for login
    public sealed class LoginUserCommandHandler: IRequestHandler<LoginUserCommand, LoginResponse>
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

        public async Task<LoginResponse> Handle(LoginUserCommand request,CancellationToken cancellationToken)
        {
            // Normalize email before searching
            var email = request.Email.Trim().ToLowerInvariant();

            // Find the user.
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            // Use the same error for unknown email/password. This avoids revealing whether an account exists.
            if (user is null)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            // Check whether the account is active.
            if (!user.IsActive)
            {
                throw new UnauthorizedException("This account is inactive.");
            }

            // Verify the supplied password against the stored hash.
            var passwordValid = _passwordHasher.Verify(request.Password,user.PasswordHash);

            if (!passwordValid)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            // Generate short-lived access token.
            var accessToken = _jwtService.GenerateAccessToken(user);
            var accessTokenExpiresAt =DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

            // Generate long-lived refresh token.
            var refreshTokenValue = _refreshTokenGenerator.Generate();
            var refreshToken = new RefreshToken(user.Id, refreshTokenValue, DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays));

            // Persist refresh token.
            await _refreshTokenRepository.AddAsync(refreshToken,cancellationToken);

            // Return authentication result.         
            return new LoginResponse(user.Id,accessToken,refreshTokenValue,accessTokenExpiresAt);
        }
    }
}
