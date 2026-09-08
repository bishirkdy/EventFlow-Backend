namespace EventFlow.Identity.Application.DTOs.Authentication
{
    //Defines what the registration use case returns
    public sealed record RegisterUserResponse
    {
        public Guid Id { get; init; }
        public string UserName { get; init; } = null!;
        public string Email { get; init; } = null!;
    }
}
