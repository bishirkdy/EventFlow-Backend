namespace EventFlow.Identity.Application.DTOs
{
    // Contains the password-reset token and the new password.
    public record ResetPasswordDto(
        string Token,
        string NewPassword
    );
}
