namespace EventFlow.Operations.Application.Abstractions;

public sealed record IdentityUserSummary(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string DisplayName);
