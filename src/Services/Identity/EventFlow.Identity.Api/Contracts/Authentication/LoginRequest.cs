namespace EventFlow.Identity.Api.Contracts.Authentication
{
    // Defines the HTTP request contract for user login.
    public sealed record LoginRequest(string Email,string Password);
}
