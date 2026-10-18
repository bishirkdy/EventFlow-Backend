namespace EventFlow.Identity.Api.Contracts.Authentication;

public sealed record RegisterUserRequest(string UserName, string Email, string Password, string FirstName, string LastName);
