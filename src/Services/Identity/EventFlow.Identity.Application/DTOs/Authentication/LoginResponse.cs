namespace EventFlow.Identity.Application.DTOs.Authentication
{
    public sealed record LoginResponse(Guid UserId,string AccessToken,string RefreshToken,DateTime AccessTokenExpiresAt);
}
