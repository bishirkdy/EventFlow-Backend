namespace EventFlow.Identity.Application.DTOs
{
    // Contains the newly issued access and refresh tokens.
    public record RefreshTokenResponseDto(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresAt
    );
}
