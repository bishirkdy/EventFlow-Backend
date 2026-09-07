

using MediatR;

namespace EventFlow.Identity.Application.Commands.LogoutUser
{
    public record LogoutUserCommand(string RefreshToken) : IRequest;
}
