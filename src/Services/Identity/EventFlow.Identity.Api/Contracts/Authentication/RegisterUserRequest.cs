namespace EventFlow.Identity.Api.Contracts.Authentication
{
    //Defines exactly what the HTTP API accepts.
    public sealed record RegisterUserRequest(string UserName,string Email,string Password,string FirstName,string LastName);
}
