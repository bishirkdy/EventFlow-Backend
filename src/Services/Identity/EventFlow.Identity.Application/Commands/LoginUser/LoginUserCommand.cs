using EventFlow.Identity.Application.DTOs;
using EventFlow.Identity.Application.DTOs.Authentication;
using MediatR;

namespace EventFlow.Identity.Application.Commands.Login
{
    // Represents a request to authenticate a user and return authentication details.
    public sealed record LoginUserCommand(string Email,string Password) : IRequest<LoginResponse>;
}
