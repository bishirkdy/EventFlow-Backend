namespace EventFlow.Identity.Application.DTOs
{
    // Represents the information returned for the currently authenticated user.
    public record CurrentUserResponseDto(
        Guid Id,
        string Email,
        string UserName,
        string FirstName,
        string LastName
    );
}
