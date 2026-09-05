namespace EventFlow.Identity.Application.DTOs
{
    // Identifies the user who requested a password reset.
    public record ForgotPasswordDto(
        string Email
    );
}
