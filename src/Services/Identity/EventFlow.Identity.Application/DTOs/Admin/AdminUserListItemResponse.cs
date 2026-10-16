namespace EventFlow.Identity.Application.DTOs.Admin;

public sealed record AdminUserListItemResponse(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    bool IsPlatformAdmin,
    DateTime CreatedAt);
