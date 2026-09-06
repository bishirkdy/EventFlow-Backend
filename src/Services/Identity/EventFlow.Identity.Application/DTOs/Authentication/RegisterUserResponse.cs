namespace EventFlow.Identity.Application.DTOs.Authentication
{
    //Defines what the registration use case returns
    public sealed record RegisterUserResponse(Guid UserId,string UserName,string Email);
}
