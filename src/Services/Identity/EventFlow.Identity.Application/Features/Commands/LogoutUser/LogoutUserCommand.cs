using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.LogoutUser;

public sealed record LogoutUserCommand(string? RefreshToken) : IRequest;
