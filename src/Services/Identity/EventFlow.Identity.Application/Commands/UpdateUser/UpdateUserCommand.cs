using EventFlow.Identity.Application.DTOs;
using MediatR;

namespace EventFlow.Identity.Application.Commands.UpdateUser
{
    // Represents a request to update an existing user's profile information.
    public record UpdateUserCommand(Guid UserId,UpdateUserDto User) : IRequest<UserResponseDto>;
}
