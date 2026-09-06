
using System.Security.Cryptography;

namespace EventFlow.Identity.Infrastructure.Services
{
    public static class RefreshTokenGenerator
    {
        public static string Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(bytes);
        }
    }
}
