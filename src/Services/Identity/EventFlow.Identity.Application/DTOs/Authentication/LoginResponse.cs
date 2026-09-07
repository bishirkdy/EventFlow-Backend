namespace EventFlow.Identity.Application.DTOs.Authentication
{
    //Login response dto
    public sealed record LoginResponse(Guid UserId,string AccessToken,string RefreshToken,DateTime AccessTokenExpiresAt);
}
