using System.ComponentModel.DataAnnotations;

namespace EventFlow.Identity.Api.Contracts.Authentication
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
