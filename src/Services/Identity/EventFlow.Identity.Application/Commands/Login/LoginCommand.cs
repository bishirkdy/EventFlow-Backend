using EventFlow.Identity.Application.DTOs;
using MediatR;

namespace EventFlow.Identity.Application.Commands.Login
{
    // Represents a request to authenticate a user and return authentication details.
    public record LoginCommand(LoginDto Login) : IRequest<LoginResponseDto>;
}
