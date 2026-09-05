namespace EventFlow.Identity.Application.DTOs
{
    // Contains the user profile information that can be updated.   
    public record UpdateUserDto(
        string FirstName,
        string LastName
    );
}
