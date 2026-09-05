namespace EventFlow.Identity.Application.DTOs
{
    // Represents the public user information returned by the Identity service.
    public record UserResponseDto(
        Guid Id,
        string Email,
        string UserName,
        string FirstName,
        string LastName,
        bool IsActive
    );
}
