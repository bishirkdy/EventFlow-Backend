using EventFlow.Identity.Application.Abstractions.Services;
using EventFlow.Identity.Application.Configuration;
using EventFlow.Identity.Domain.Entities;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace EventFlow.Identity.Infrastructure.Services
{
    public sealed class JwtService : IJwtService
    {
        private readonly JwtOptions _options;

        public JwtService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        // Creates JWT access tokens.
        public string GenerateAccessToken(User user)
        {
            // Create claims that identify the authenticated user.
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName)
        };

            // Convert the secret into a signing key.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            // Define how the token will be signed.
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT.
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes),
                signingCredentials: credentials);

            // Serialize JWT into the string returned to the client.
            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
