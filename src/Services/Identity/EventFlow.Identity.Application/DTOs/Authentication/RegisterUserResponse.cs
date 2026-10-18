namespace EventFlow.Identity.Application.DTOs.Authentication;

public sealed record RegisterUserResponse
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
}
