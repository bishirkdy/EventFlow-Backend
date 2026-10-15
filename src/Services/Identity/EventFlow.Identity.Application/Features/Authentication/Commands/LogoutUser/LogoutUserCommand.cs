using MediatR;

namespace EventFlow.Identity.Application.Features.Authentication.Commands.LogoutUser;

public sealed record LogoutUserCommand(string? RefreshToken) : IRequest;
