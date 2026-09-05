namespace EventFlow.Identity.Application.DTOs
{
    // Contains the credentials required to authenticate a user.
    public record LoginDto(string Email , string Password);
}
