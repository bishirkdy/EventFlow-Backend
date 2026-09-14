

namespace EventFlow.Identity.Application.DTOs.Authentication
{
    public sealed record UserProfileResponse(
        Guid Id,
        string UserName,
        string Email,
        string FirstName,
        string LastName
    );
}
