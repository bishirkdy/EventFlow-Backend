

using MediatR;

namespace EventFlow.Identity.Application.Commands.LogoutUser
{
    //Command for logout
    public sealed record LogoutUserCommand(string RefreshToken) : IRequest;
}
