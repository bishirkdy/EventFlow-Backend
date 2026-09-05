namespace EventFlow.Identity.Application.DTOs
{
    // Contains the refresh token required to obtain a new access token.
    public record RefreshTokenDto(
        string RefreshToken
    );
}
