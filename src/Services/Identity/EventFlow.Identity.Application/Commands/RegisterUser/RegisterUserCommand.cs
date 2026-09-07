using EventFlow.Identity.Application.DTOs;
using EventFlow.Identity.Application.DTOs.Authentication;
using MediatR;

namespace EventFlow.Identity.Application.Commands.RegisterUser
{
    // Represents a request to register a new user and returns the created UserId.
    public sealed record RegisterUserCommand(string UserName,string Email,string Password,string FirstName,string LastName) : IRequest<RegisterUserResponse>;
}
