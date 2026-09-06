
using EventFlow.Identity.Application.Abstractions.Services;
using System.Security.Cryptography;

namespace EventFlow.Identity.Infrastructure.Services
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(bytes);
        }
    }
}
