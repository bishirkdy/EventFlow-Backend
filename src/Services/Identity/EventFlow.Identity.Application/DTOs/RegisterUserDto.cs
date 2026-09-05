namespace EventFlow.Identity.Application.DTOs
{
    public record RegisterUserDto(
        string Email,
        string UserName,
        string FirstName,
        string LastName
    );
}
