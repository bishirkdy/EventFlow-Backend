namespace EventFlow.Identity.Application.DTOs.Users;

public sealed record UserSummaryResponse(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string DisplayName);
