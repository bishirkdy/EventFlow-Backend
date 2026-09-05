namespace EventFlow.Identity.Application.DTOs
{
    // Contains authentication tokens and the authenticated user's information.
    public record LoginResponseDto(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresAt,
        UserResponseDto User
    );
}
