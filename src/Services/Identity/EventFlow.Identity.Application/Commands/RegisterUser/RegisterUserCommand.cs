using EventFlow.Identity.Application.DTOs;
using MediatR;

namespace EventFlow.Identity.Application.Commands.RegisterUser
{
    // Represents a request to register a new user and returns the created UserId.
    public record RegisterUserCommand(RegisterUserDto User) : IRequest<Guid>;
}
