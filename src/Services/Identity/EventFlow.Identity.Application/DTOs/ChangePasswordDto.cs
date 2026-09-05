namespace EventFlow.Identity.Application.DTOs
{
    // Contains the current and new passwords required to change a password.
    public record ChangePasswordDto(
        string CurrentPassword,
        string NewPassword
    );
}
